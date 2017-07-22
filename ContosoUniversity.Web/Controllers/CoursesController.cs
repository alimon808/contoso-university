using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Department> _departmentRepo;
        private ApplicationContext _context;

        public CoursesController(ApplicationContext context, IRepository<Course> courseRepo, IRepository<Department> departmentRepo)
        {
            _context = context;
            _courseRepo = courseRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var schoolContext = _courseRepo.GetAll().Include(c => c.Department);
            return View(await schoolContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepo.GetAll()
                .Include(c => c.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);
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
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Credits,DepartmentID,Title")] Course course)
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

            var course = await _courseRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _courseRepo.GetAll().SingleOrDefaultAsync(c => c.CourseID == id);

            if (await TryUpdateModelAsync<Course>(courseToUpdate, "", c => c.Credits, c => c.DepartmentID, c => c.Title))
            {
                try
                {
                    await _courseRepo.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, see your system administrator");
                }
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepo.GetAll()
                .Include(c => c.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepo.GetAll().SingleOrDefaultAsync(m => m.CourseID == id);
            _courseRepo.Delete(course);
            await _courseRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                // todo: add ExecuteSqlCommandAsync to repo
                ViewData["RowsAffected"] = await _context.Database.ExecuteSqlCommandAsync("UPDATE Course SET Credits = Credits * {0}", parameters: multiplier);
            }
            return View();
        }

        private bool CourseExists(int id)
        {
            return _courseRepo.GetAll().Any(e => e.CourseID == id);
        }
    }
}
