using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Entities;
using System;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Department> _departmentRepo;
        private readonly IModelBindingHelperAdaptor _modelBindingHelperAdaptor;

        public CoursesController(UnitOfWork<ApplicationContext> unitOfWork, IModelBindingHelperAdaptor modelBindingHelperAdaptor)
        {
            _courseRepo = unitOfWork.CourseRepository;
            _departmentRepo = unitOfWork.DepartmentRepository;
            _modelBindingHelperAdaptor = modelBindingHelperAdaptor;
        }

        public async Task<IActionResult> Index()
        {
            var courses = _courseRepo.GetAll().Include(c => c.Department);
            return View(await courses.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepo.GetAll()
                .Include(c => c.Department)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _departmentRepo.GetAll()
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "ID", "Name", selectedDepartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseNumber,Credits,DepartmentID,Title")] Course course)
        {
            if (ModelState.IsValid)
            {
                await _courseRepo.AddAsync(course);
                await _courseRepo.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepo.GetAll().AsGatedNoTracking().SingleOrDefaultAsync(m => m.ID == id);
            if (course == null)
            {
                return NotFound();
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        //todo: refactor - remove TryUpdateModelAsync to perform unit test
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _courseRepo.GetAll().SingleOrDefaultAsync(c => c.ID == id);

            if (courseToUpdate == null)
            {
                return NotFound();
            }

            if (await _modelBindingHelperAdaptor.TryUpdateModelAsync<Course>(this, courseToUpdate, "", c => c.Credits, c => c.DepartmentID, c => c.Title))
            {
                try
                {
                    courseToUpdate.ModifiedDate = DateTime.UtcNow;
                    await _courseRepo.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, see your system administrator");
                }
            }

            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepo.Get(id.Value)
                .Include(c => c.Department)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepo.Get(id.Value).SingleOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            _courseRepo.Delete(course);
            await _courseRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier > 1)
            {
                ViewData["RowsAffected"] = await _courseRepo.ExecuteSqlCommandAsync($"UPDATE Contoso.Course SET Credits = Credits * {multiplier}");
            }
            else
            {
                ModelState.AddModelError("InvalidMultiplier", "Multiplier must be greater than 1");
            }

            return View();
        }

        private bool CourseExists(int id)
        {
            return _courseRepo.GetAll().Any(e => e.ID == id);
        }
    }
}
