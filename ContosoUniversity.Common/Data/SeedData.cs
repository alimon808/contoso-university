using Microsoft.Extensions.Logging;
using ContosoUniversity.Data.Entities;
using System.Linq;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Common.Data
{
    public class SeedData<TContext> where TContext : DbContext
    {
        private readonly UnitOfWork<TContext> _unitOfWork;
        private readonly ILogger _logger;
        private readonly SampleData _data;

        public SeedData(ILogger logger, UnitOfWork<TContext> unitOfWork, SampleData data)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _data = data;
        }

        public void Initialize()
        {
            InitializeInstructors();
            InitializeDeparments();
            InitializeStudents();
            InitializeCourses();
            InitializeOfficeAssignment();
            InitializeCourseAssignment();
            InitializeEnrollments();

            //if (_data.SaveToExternalFile)
            //{
            //    SaveToJson();
            //}
        }

        //private void SaveToJson()
        //{
        //    var file = $"{Directory.GetCurrentDirectory()}\\data\\sample-data.json";
        //    if (File.Exists(file))
        //    {
        //        File.Delete(file);
        //    }
        //    JsonSerializer serializer = new JsonSerializer
        //    {
        //        NullValueHandling = NullValueHandling.Ignore,
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    };
        //    using (StreamWriter sw = new StreamWriter(file))
        //    using (JsonWriter writer = new JsonTextWriter(sw))
        //    {
        //        serializer.Serialize(writer, _data);
        //    }
        //}

        private void InitializeEnrollments()
        {
            if (IsInitialized(_unitOfWork.EnrollmentRepository.GetAll()))
            {
                return;
            }

            InitializeStudents();
            InitializeCourses();

            var students = _unitOfWork.StudentRepository.GetAll().ToArray();
            var courses = _unitOfWork.CourseRepository.GetAll().ToArray();
            var enrollments = _data.Enrollments.Select(e => new Enrollment
            {
                CourseID = e.CourseID,
                StudentID = e.StudentID,
                Grade = e.Grade
            });

            _logger.LogInformation("Seeding enrollment table");
            foreach (Enrollment e in enrollments)
            {
                _unitOfWork.EnrollmentRepository.Add(e);
            }

            _unitOfWork.Commit();
        }

        private void InitializeCourseAssignment()
        {
            if (IsInitialized(_unitOfWork.CourseAssignmentRepository.GetAll()))
            {
                return;
            }

            InitializeInstructors();
            InitializeCourses();

            var instructors = _unitOfWork.InstructorRepository.GetAll().ToArray();
            var courses = _unitOfWork.CourseRepository.GetAll().ToArray();

            var courseAssignments = _data.CourseAssignments.Select(ca => new CourseAssignment
            {
                InstructorID = ca.InstructorID,
                CourseID = ca.CourseID
            });

            _logger.LogInformation("Seeding CourseAssignment table");
            foreach (CourseAssignment c in courseAssignments)
            {
                _unitOfWork.CourseAssignmentRepository.Add(c);
            }

            _unitOfWork.Commit();
        }

        private void InitializeOfficeAssignment()
        {
            if (IsInitialized(_unitOfWork.OfficeAssignmentRepository.GetAll()))
            {
                return;
            }

            InitializeInstructors();

            var instructors = _unitOfWork.InstructorRepository.GetAll().ToArray();
            var officeAssignments = _data.OfficeAssignments.Select(oa => new OfficeAssignment
            {
                InstructorID = oa.InstructorID,
                Location = oa.Location
            });

            _logger.LogInformation("Seeding OfficeAssignment table");
            foreach (OfficeAssignment o in officeAssignments)
            {
                _unitOfWork.OfficeAssignmentRepository.Add(o);
            }

            _unitOfWork.Commit();
        }

        private void InitializeCourses()
        {
            if (IsInitialized(_unitOfWork.CourseRepository.GetAll()))
            {
                return;
            }

            InitializeDeparments();

            var departments = _unitOfWork.DepartmentRepository.GetAll().ToArray();
            var courses = _data.Courses.Select(c => new Course
            {
                CourseNumber = c.CourseNumber,
                Title = c.Title,
                Credits = c.Credits,
                DepartmentID = c.DepartmentID
            });

            _logger.LogInformation("Seeding Course table");
            foreach (Course c in courses)
            {
                _unitOfWork.CourseRepository.Add(c);
            }

            _unitOfWork.Commit();
        }

        private void InitializeStudents()
        {
            if (IsInitialized(_unitOfWork.StudentRepository.GetAll()))
            {
                return;
            }

            var students = _data.Students.Select(s => new Student
            {
                FirstMidName = s.FirstMidName,
                LastName = s.LastName,
                EnrollmentDate = s.EnrollmentDate
            });

            _logger.LogInformation("Seeding student table");
            foreach (Student s in students)
            {
                _unitOfWork.StudentRepository.Add(s);
            }

            _unitOfWork.Commit();

        }

        private void InitializeDeparments()
        {
            if (IsInitialized(_unitOfWork.DepartmentRepository.GetAll()))
            {
                return;
            }

            InitializeInstructors();

            var instructors = _unitOfWork.InstructorRepository.GetAll().ToArray();
            var departments = _data.Departments.Select(d => new Department
            {
                Name = d.Name,
                Budget = d.Budget,
                StartDate = d.StartDate,
                InstructorID = d.InstructorID
            });

            _logger.LogInformation("Seeding department table");
            foreach (Department d in departments)
            {
                _unitOfWork.DepartmentRepository.Add(d);
            }

            _unitOfWork.Commit();
        }

        private void InitializeInstructors()
        {
            if (IsInitialized(_unitOfWork.InstructorRepository.GetAll()))
            {
                return;
            }

            var instructors = _data.Instructors.Select(i => new Instructor
            {
                FirstMidName = i.FirstMidName,
                LastName = i.LastName,
                HireDate = i.HireDate
            });

            _logger.LogInformation("Seeding instructor table");
            foreach (Instructor i in instructors)
            {
                _unitOfWork.InstructorRepository.Add(i);
            }

            _unitOfWork.Commit();
        }

        private bool IsInitialized(IQueryable<BaseEntity> query) => query.Count() > 0;
    }
}
