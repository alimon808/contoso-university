using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ContosoUniversity.Data.Entities;
using System.Linq;
using ContosoUniversity.Data.Enums;

namespace ContosoUniversity.Data
{
    public class SeedData
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SeedData(ILogger logger, UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
        }

        private void InitializeEnrollments()
        {
            var studentCount = _unitOfWork.StudentRepository.GetAll().Count();
            if (studentCount == 0)
            {
                InitializeStudents();
            }

            var courseCount = _unitOfWork.CourseRepository.GetAll().Count();
            if (courseCount == 0)
            {
                InitializeCourses();
            }

            var students = _unitOfWork.StudentRepository.GetAll().ToArray();
            var courses = _unitOfWork.CourseRepository.GetAll().ToArray();
            var enrollments = new Enrollment[]
            {
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).ID,
                    Grade = Grade.A
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).ID,
                    Grade = Grade.C
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).ID,
                    Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Calculus" ).ID,
                    Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).ID,
                    Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Composition" ).ID,
                Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Anand").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).ID
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Anand").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics").ID,
                    Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Barzdukas").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry").ID,
                    Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Li").ID,
                    CourseID = courses.Single(c => c.Title == "Composition").ID,
                    Grade = Grade.B
                    },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Justice").ID,
                    CourseID = courses.Single(c => c.Title == "Literature").ID,
                    Grade = Grade.B
                }
            };

            _logger.LogInformation("Seeding enrollment table");
            foreach (Enrollment e in enrollments)
            {
                _unitOfWork.EnrollmentRepository.Add(e);
            }

            _unitOfWork.Commit();
        }

        private void InitializeCourseAssignment()
        {
            var instructorsCount = _unitOfWork.InstructorRepository.GetAll().Count();
            if (instructorsCount == 0)
            {
                InitializeInstructors();
            }

            var courseCount = _unitOfWork.CourseRepository.GetAll().Count();
            if (courseCount == 0)
            {
                InitializeCourses();
            }

            var instructors = _unitOfWork.InstructorRepository.GetAll().ToArray();
            var courses = _unitOfWork.CourseRepository.GetAll().ToArray();

            var courseAssignments = new CourseAssignment[]
            {
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Calculus" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Composition" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
                },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Literature" ).ID,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
                }
            };

            _logger.LogInformation("Seeding CourseAssignment table");
            foreach (CourseAssignment c in courseAssignments)
            {
                _unitOfWork.CourseAssignmentRepository.Add(c);
            }

            _unitOfWork.Commit();
        }

        private void InitializeOfficeAssignment()
        {
            var instructorsCount = _unitOfWork.InstructorRepository.GetAll().Count();
            if (instructorsCount == 0)
            {
                InitializeInstructors();
            }

            var instructors = _unitOfWork.InstructorRepository.GetAll().ToArray();
            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment { InstructorID = instructors.Single( i => i.LastName == "Fakhouri").ID, Location = "Smith 17" },
                new OfficeAssignment { InstructorID = instructors.Single( i => i.LastName == "Harui").ID, Location = "Gowan 27" },
                new OfficeAssignment { InstructorID = instructors.Single( i => i.LastName == "Kapoor").ID, Location = "Thompson 304" },
            };

            _logger.LogInformation("Seeding OfficeAssignment table");
            foreach (OfficeAssignment o in officeAssignments)
            {
                _unitOfWork.OfficeAssignmentRepository.Add(o);
            }

            _unitOfWork.Commit();
        }

        private void InitializeCourses()
        {
            var departmentCount = _unitOfWork.DepartmentRepository.GetAll().Count();
            if (departmentCount == 0)
            {
                InitializeDeparments();
            }
            var departments = _unitOfWork.DepartmentRepository.GetAll().ToArray();
            var courses = new Course[]
            {
                new Course {CourseNumber = 1050, Title = "Chemistry", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Engineering").ID
                },
                new Course {CourseNumber = 4022, Title = "Microeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").ID
                },
                new Course {CourseNumber = 4041, Title = "Macroeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").ID
                },
                new Course {CourseNumber = 1045, Title = "Calculus", Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").ID
                },
                new Course {CourseNumber = 3141, Title = "Trigonometry", Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").ID
                },
                new Course {CourseNumber = 2021, Title = "Composition", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "English").ID
                },
                new Course {CourseNumber = 2042, Title = "Literature", Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "English").ID
                }
            };

            _logger.LogInformation("Seeding Course table");
            foreach (Course c in courses)
            {
                _unitOfWork.CourseRepository.Add(c);
            }

            _unitOfWork.Commit();
        }

        private void InitializeStudents()
        {
            var students = new Student[]
            {
                new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")},
                new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01")},
                new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2003-09-01")},
                new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2002-09-01")},
                new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2002-09-01")},
                new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2001-09-01")},
                new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2003-09-01")},
                new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2005-09-01")}
            };

            _logger.LogInformation("Seeding student table");
            foreach (Student s in students)
            {
                _unitOfWork.StudentRepository.Add(s);
            }

            _unitOfWork.Commit();
        }

        private void InitializeDeparments()
        {
            var departmentCount =  _unitOfWork.DepartmentRepository.GetAll().Count();
            if (departmentCount > 0)
            {
                return;
            }

            var instructorCount = _unitOfWork.InstructorRepository.GetAll().Count();
            if (instructorCount == 0)
            {
                InitializeInstructors();
            }

            var instructors = _unitOfWork.InstructorRepository.GetAll().ToArray();
            var departments = new Department[]
            {
                new Department { Name = "English",     Budget = 350000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Fakhouri").ID },
                new Department { Name = "Engineering", Budget = 350000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Harui").ID },
                new Department { Name = "Economics",   Budget = 100000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Kapoor").ID }
            };

            _logger.LogInformation("Seeding department table");
            foreach (Department d in departments)
            {
                _unitOfWork.DepartmentRepository.Add(d);
            }
            _unitOfWork.Commit();
        }

        private void InitializeInstructors()
        {
            var instructorCount = _unitOfWork.InstructorRepository.GetAll().Count();
            if (instructorCount > 0)
            {
                return;
            }

            var instructors = new Instructor[]
            {
                new Instructor { FirstMidName = "Kim",     LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { FirstMidName = "Fadi",    LastName = "Fakhouri", HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { FirstMidName = "Roger",   LastName = "Harui", HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { FirstMidName = "Candace", LastName = "Kapoor", HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { FirstMidName = "Roger",   LastName = "Zheng", HireDate = DateTime.Parse("2004-02-12") }
            };

            _logger.LogInformation("Seeding instructor table");
            foreach (Instructor i in instructors)
            {
                _unitOfWork.InstructorRepository.Add(i);
            }

            _unitOfWork.Commit();
        }
    }
}
