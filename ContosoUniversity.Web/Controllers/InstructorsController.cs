using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity.Web.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IRepository<Department> _departmentRepo;
        private readonly IRepository<Instructor> _instructorRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<CourseAssignment> _courseAssignmentRepo;
        private readonly IModelBindingHelperAdaptor _modelBindingHelperAdaptor;

        public InstructorsController(UnitOfWork<ApplicationContext> unitOfWork,
            IModelBindingHelperAdaptor modelBindingHelperAdaptor)
        {
            _instructorRepo = unitOfWork.InstructorRepository;
            _departmentRepo = unitOfWork.DepartmentRepository;
            _courseRepo = unitOfWork.CourseRepository;
            _courseAssignmentRepo = unitOfWork.CourseAssignmentRepository;
            _modelBindingHelperAdaptor = modelBindingHelperAdaptor;
        }

        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData() { };
            viewModel.Instructors = await _instructorRepo.GetAll()
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Student)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                .AsGatedNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                ViewData["InstructorID"] = id.Value;
                Instructor instructor = viewModel.Instructors.Where(i => i.ID == id.Value).Single();
                viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            if (courseID != null)
            {
                ViewData["CourseID"] = courseID.Value;
                viewModel.Enrollments = viewModel.Courses.Where(x => x.ID == courseID).Single().Enrollments;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _instructorRepo.Get(id.Value).SingleOrDefaultAsync();

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        public IActionResult Create()
        {
            var instructor = new Instructor() { };
            instructor.CourseAssignments = new List<CourseAssignment>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstMidName,HireDate,LastName,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { InstructorID = instructor.ID, CourseID = int.Parse(course) };
                    instructor.CourseAssignments.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                await _instructorRepo.AddAsync(instructor);
                await _instructorRepo.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _instructorRepo.Get(id.Value)
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                .AsGatedNoTracking()
                .SingleOrDefaultAsync();

            if (instructor == null)
            {
                return NotFound();
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _instructorRepo.GetAll()
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                .SingleOrDefaultAsync(s => s.ID == id);

            if (await _modelBindingHelperAdaptor.TryUpdateModelAsync<Instructor>(this, instructorToUpdate, "", i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }

                UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                try
                {
                    instructorToUpdate.ModifiedDate = DateTime.UtcNow;
                    await _instructorRepo.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, see your system administrator.");
                }
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _instructorRepo.Get(id.Value).SingleOrDefaultAsync();

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Instructor instructor = await _instructorRepo.GetAll()
                .Include(i => i.CourseAssignments)
                .SingleAsync(m => m.ID == id);

            var departments = await _departmentRepo.GetAll().Where(d => d.InstructorID == id).ToListAsync();
            departments.ForEach(d => d.InstructorID = null);

            _instructorRepo.Delete(instructor);

            await _instructorRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructorToUpdate.CourseAssignments.Select(c => c.Course.ID));
            foreach (var course in _courseRepo.GetAll())
            {
                if (selectedCoursesHS.Contains(course.ID.ToString()))
                {
                    if (!instructorCourses.Contains(course.ID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.ID });
                    }
                }
                else
                {
                    if (instructorCourses.Contains(course.ID))
                    {
                        CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.SingleOrDefault(i => i.CourseID == course.ID);
                        _courseAssignmentRepo.Delete(courseToRemove);
                    }
                }
            }
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = _courseRepo.GetAll();
            var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.ID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.ID)
                });
            }
            ViewData["Courses"] = viewModel;
        }

        private bool InstructorExists(int id)
        {
            return _instructorRepo.GetAll().Any(e => e.ID == id);
        }
    }
}
