using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data.Entities;
using System;
using ContosoUniversity.ViewModels;
using ContosoUniversity.Common.Interfaces;
using AutoMapper;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity.Web.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IRepository<Department> _departmentRepo;
        private readonly IRepository<Instructor> _instructorRepo;
        private readonly IModelBindingHelperAdaptor _modelBindingHelperAdaptor;
        private readonly IMapper _mapper;

        public DepartmentsController(UnitOfWork<ApplicationContext> unitOfWork,
                                     IModelBindingHelperAdaptor modelBindingHelperAdaptor,
                                     IMapper mapper)
        {
            _departmentRepo = unitOfWork.DepartmentRepository;
            _instructorRepo = unitOfWork.InstructorRepository;
            _modelBindingHelperAdaptor = modelBindingHelperAdaptor;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var query = _departmentRepo.GetAll()
                .Include(d => d.Administrator)
                .Select(d => _mapper.Map<DepartmentDetailsViewModel>(d));

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _departmentRepo.Get(id.Value)
                .Include(d => d.Administrator)
                .Select(d => _mapper.Map<DepartmentDetailsViewModel>(d))
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

            if (vm == null)
            {
                return NotFound();
            }

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
                var department = _mapper.Map<Department>(vm);
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

            var vm = _mapper.Map<DepartmentEditViewModel>(department);

            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", department.InstructorID);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var departmentToUpdate = await _departmentRepo.Get(vm.ID)
                .Include(i => i.Administrator)
                .SingleOrDefaultAsync();

            if (departmentToUpdate == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.  The department was deleted by another user.");
                ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", vm.InstructorID);
                return View(vm);
            }

            try
            {
                departmentToUpdate = _mapper.Map(vm, departmentToUpdate);
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

            ViewData["InstructorID"] = new SelectList(_instructorRepo.GetAll(), "ID", "FullName", vm.InstructorID);
            return View(vm);
        }

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentRepo.Get(id).FirstOrDefaultAsync();

            if (department == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                _departmentRepo.Delete(department);
                await _departmentRepo.SaveChangesAsync();
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
