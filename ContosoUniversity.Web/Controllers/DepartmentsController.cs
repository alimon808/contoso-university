using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;
using System;
using ContosoUniversity.Web;
using ContosoUniversity.ViewModels;
using System.Collections.Generic;

namespace ContosoUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IRepository<Department> _departmentRepo;
        private readonly IRepository<Instructor> _instructorRepo;
        private readonly IModelBindingHelperAdaptor _modelBindingHelperAdaptor;

        public DepartmentsController(IRepository<Department> departmentRepo, IRepository<Instructor> instructorRepo, IModelBindingHelperAdaptor modelBindingHelperAdaptor)
        {
            _departmentRepo = departmentRepo;
            _instructorRepo = instructorRepo;
            _modelBindingHelperAdaptor = modelBindingHelperAdaptor;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentRepo.GetAll()
                .Include(d => d.Administrator)
                .ToListAsync();

            var vm = new List<DepartmentDetailsViewModel>();
            departments.ForEach(d => vm.Add(new DepartmentDetailsViewModel
            {
                ID = d.ID,
                Name = d.Name,
                Budget = d.Budget,
                StartDate = d.StartDate,
                Administrator = d.Administrator?.FullName,
            }));

            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepo.Get(id.Value)
                .Include(d => d.Administrator)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();
            
            if (department == null)
            {
                return NotFound();
            }

            var vm = new DepartmentDetailsViewModel
            {
                ID = department.ID,
                Name = department.Name,
                Budget = department.Budget,
                StartDate = department.StartDate,
                Administrator = department.Administrator?.FullName
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var department = new Department {
                    Name = vm.Name,
                    Budget = vm.Budget,
                    StartDate = vm.StartDate,
                    InstructorID = vm.InstructorID
                };
                await _departmentRepo.AddAsync(department);
                await _departmentRepo.SaveChangesAsync();
                return RedirectToAction("Index", new { newid = department.ID });
            }

            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", vm.InstructorID);
            return View(vm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepo.Get(id.Value)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

            if (department == null)
            {
                return NotFound();
            }

            var vm = new DepartmentEditViewModel
            {
                ID = id.Value,
                Name = department.Name,
                Budget = department.Budget,
                StartDate = department.StartDate,
                RowVersion = department.RowVersion?.ToString(),
                Administrator = department?.Administrator?.FullName,
                InstructorID = department.InstructorID.Value
            };

            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", department.InstructorID);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _departmentRepo.Get(id.Value)
                .Include(i => i.Administrator)
                .SingleOrDefaultAsync();

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await _modelBindingHelperAdaptor.TryUpdateModelAsync(this, deletedDepartment);
                ModelState.AddModelError(string.Empty, "Unable to save changes.  The department was deleted by another user.");
                ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }
            
            if (await _modelBindingHelperAdaptor.TryUpdateModelAsync<Department>(this, departmentToUpdate, "", s => s.Name, s => s.Budget, s => s.InstructorID, s => s.StartDate))
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

            var department = await _departmentRepo.Get(id.Value)
                .Include(d => d.Administrator)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

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
                if (await _departmentRepo.GetAll().AnyAsync(m => m.ID == department.ID))
                {
                    _departmentRepo.Delete(department);
                    await _departmentRepo.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.ID });
            }
        }

        private bool DepartmentExists(int id)
        {
            return _departmentRepo.GetAll().Any(e => e.ID == id);
        }
    }
}
