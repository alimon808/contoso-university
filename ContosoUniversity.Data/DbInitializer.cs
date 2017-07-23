using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Enums;
using System;
using System.Linq;

namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            ApplicationContext context = (new ApplicationContextFactory().Create());
            context.Database.EnsureCreated();
            if (context.Students.Any())
            {
                return;
            }

            StudentInitialize(context);
            InstructorInitialize(context);
            DepartmentInitialize(context);
            CourseInitialize(context);
            OfficeAssignmentInitialize(context);
            CourseAssignmentInitialize(context);
            EnrollmentInitialize(context);
        }

        private static void CourseAssignmentInitialize(ApplicationContext context)
        {
            var instructors = context.Instructors.ToArray();
            var courses = context.Courses.ToArray();
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

            foreach (CourseAssignment c in courseAssignments)
            {
                context.CourseAssignments.Add(c);
            }

            SaveChanges(context);
        }

        private static void OfficeAssignmentInitialize(ApplicationContext context)
        {
            var instructors = context.Instructors.ToArray();
            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment { ID = instructors.Single( i => i.LastName == "Fakhouri").ID, Location = "Smith 17" },
                new OfficeAssignment { ID = instructors.Single( i => i.LastName == "Harui").ID, Location = "Gowan 27" },
                new OfficeAssignment { ID = instructors.Single( i => i.LastName == "Kapoor").ID, Location = "Thompson 304" },
            };

            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o);
            }

            SaveChanges(context);
        }

        private static void StudentInitialize(ApplicationContext context)
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

            foreach (Student s in students)
            {
                context.Students.Add(s);
            }

            SaveChanges(context);
        }

        private static void InstructorInitialize(ApplicationContext context)
        {
            var instructors = new Instructor[]
            {
                new Instructor { FirstMidName = "Kim",     LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { FirstMidName = "Fadi",    LastName = "Fakhouri", HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { FirstMidName = "Roger",   LastName = "Harui", HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { FirstMidName = "Candace", LastName = "Kapoor", HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { FirstMidName = "Roger",   LastName = "Zheng", HireDate = DateTime.Parse("2004-02-12") }
            };

            foreach (Instructor i in instructors)
            {
                context.Instructors.Add(i);
            }

            SaveChanges(context);
        }

        private static void DepartmentInitialize(ApplicationContext context)
        {
            var instructors = context.Instructors.ToArray();
            var departments = new Department[]
            {
                new Department { Name = "English",     Budget = 350000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Fakhouri").ID },
                new Department { Name = "Engineering", Budget = 350000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Harui").ID },
                new Department { Name = "Economics",   Budget = 100000, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = instructors.Single( i => i.LastName == "Kapoor").ID }
            };

            foreach (Department d in departments)
            {
                context.Departments.Add(d);
            }
            SaveChanges(context);
        }
        private static void CourseInitialize(ApplicationContext context)
        {
            var departments = context.Departments.ToArray();
            var courses = new Course[]
            {
                new Course {CourseNumber = 1050, Title = "Chemistry", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Engineering").DepartmentID
                },
                new Course {CourseNumber = 4022, Title = "Microeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID
                },
                new Course {CourseNumber = 4041, Title = "Macroeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID
                },
                new Course {CourseNumber = 1045, Title = "Calculus", Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID
                },
                new Course {CourseNumber = 3141, Title = "Trigonometry", Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID
                },
                new Course {CourseNumber = 2021, Title = "Composition", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "English").DepartmentID
                },
                new Course {CourseNumber = 2042, Title = "Literature", Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "English").DepartmentID
                }
            };

            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            SaveChanges(context);
        }

        private static void EnrollmentInitialize(ApplicationContext context)
        {
            var students = context.Students.ToArray();
            var courses = context.Courses.ToArray();
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

            foreach (Enrollment e in enrollments)
            {
                context.Enrollments.Add(e);
            }

            SaveChanges(context);
        }
        private static void SaveChanges(ApplicationContext context)
        {
            context.SaveChanges();
        }
    }
}
