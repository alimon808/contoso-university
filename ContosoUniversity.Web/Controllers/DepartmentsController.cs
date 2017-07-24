using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;
using System;

namespace ContosoUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Department> _departmentRepo;
        private readonly IRepository<Instructor> _instructorRepo;

        public DepartmentsController(IRepository<Course> courseRepo, IRepository<Department> departmentRepo, IRepository<Instructor> instructorRepo)
        {
            _courseRepo = courseRepo;
            _departmentRepo = departmentRepo;
            _instructorRepo = instructorRepo;
        }

        public async Task<IActionResult> Index()
        {
            var departments = _departmentRepo.GetAll().Include(d => d.Administrator);
            return View(await departments.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string query = "SELECT * FROM Contoso.Department WHERE DepartmentID = {0}";
            var department = await _departmentRepo.GetAll()
                .FromSql(query, id)
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (ModelState.IsValid)
            {
                await _departmentRepo.AddAsync(department);
                await _departmentRepo.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", department.InstructorID);
            return View(department);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepo.GetAll()
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", department.InstructorID);
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _departmentRepo.GetAll()
                .Include(i => i.Administrator)
                .SingleOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty, "Unable to save changes.  The department was deleted by another user.");
                ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }
            
            if (await TryUpdateModelAsync<Department>(departmentToUpdate, "", s => s.Name, s => s.Budget, s => s.InstructorID, s => s.StartDate))
            {
                try
                {
                    departmentToUpdate.ModifiedDate = DateTime.UtcNow;
                    await _departmentRepo.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes.  The department was deleted by another use.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();
                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current value: {databaseValues.Budget:c}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate:d}");
                        }
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                        {
                            Instructor databaseInstructor = await _instructorRepo.GetAll().SingleOrDefaultAsync(i => i.ID == databaseValues.InstructorID);
                            ModelState.AddModelError("InstructorID", $"Current value: {databaseInstructor?.FullName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another use after you got the original valued.  "
                            + "The edit operation was canceled and the current values in the database have been displayed.  If you still want to edit this record, "
                            + "click the Save button again.  Otherwise click the back to List hyperlink.");
                        departmentToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepo.GetAll()
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete was modified by another user after you got the original values.  "
                    + "The delete operation was canceled and the current values in the database have been displayed.  If you still want to delete this "
                    + "record, click the Delete button again.  Otherwise click the Back to List hyperlink.";
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                if (await _departmentRepo.GetAll().AnyAsync(m => m.DepartmentID == department.DepartmentID))
                {
                    _departmentRepo.Delete(department);
                    await _departmentRepo.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
        }

        private bool DepartmentExists(int id)
        {
            return _departmentRepo.GetAll().Any(e => e.DepartmentID == id);
        }
    }
}
