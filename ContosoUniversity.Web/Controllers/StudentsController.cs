using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IRepository<Student> _studentRepo;
        private readonly IModelBindingHelperAdaptor _modelBindingHelperAdaptor;

        public StudentsController(UnitOfWork<ApplicationContext> unitOfWork, IModelBindingHelperAdaptor modelBindingHelperAdaptor)
        {
            _studentRepo = unitOfWork.StudentRepository;
            _modelBindingHelperAdaptor = modelBindingHelperAdaptor;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? page, string currentFilter)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "LastName_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "EnrollmentDate" ? "EnrollmentDate_desc" : "EnrollmentDate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var students = from s in _studentRepo.GetAll() select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
            }

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "LastName";
            }

            bool descending = false;

            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder.Substring(0, sortOrder.Length - 5);
                descending = true;
            }

            if (descending)
            {
                students = students.OrderByDescending(e => e.GetType().GetProperty(sortOrder).GetValue(e, null));
            }
            else
            {
                students = students.OrderBy(e => e.GetType().GetProperty(sortOrder).GetValue(e, null));
            }

            int pageSize = 3;

            return View(await PaginatedList<Student>.CreateAsync(students.AsGatedNoTracking(), page ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepo.Get(id.Value)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _studentRepo.AddAsync(student);
                    await _studentRepo.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists see your system administrator.");
            }

            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepo.Get(id.Value).SingleOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentToUpdate = await _studentRepo.Get(id.Value).SingleOrDefaultAsync();

            if (await _modelBindingHelperAdaptor.TryUpdateModelAsync<Student>(this, studentToUpdate, "", s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    studentToUpdate.ModifiedDate = DateTime.UtcNow;
                    await _studentRepo.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, see your system administrator");
                }
            }

            return View(studentToUpdate);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepo.Get(id.Value)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed.  Try again, and if the problem persists see your system administrator";
            }

            return View(student);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentRepo.Get(id)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

            if (student == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                _studentRepo.Delete(student);
                await _studentRepo.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
        }

        private bool StudentExists(int id)
        {
            return _studentRepo.GetAll().Any(e => e.ID == id);
        }
    }
}
