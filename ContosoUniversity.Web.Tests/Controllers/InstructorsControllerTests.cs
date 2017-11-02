using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Enums;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class InstructorsControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IRepository<Department>> mockDepartmentRepo;
        private readonly Mock<IRepository<Course>> mockCourseRepo;
        private readonly Mock<IRepository<CourseAssignment>> mockCourseAssignmentRepo;
        private readonly Mock<IPersonRepository<Instructor>> mockInstructorRepo;
        private readonly Mock<IModelBindingHelperAdaptor> mockModelBindingHelperAdaptor;

        InstructorsController sut;

        public InstructorsControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockDepartmentRepo = Departments().AsMockRepository();
            mockInstructorRepo = Instructors().AsMockPersonRepository();
            mockCourseRepo = Courses().AsMockRepository();
            mockCourseAssignmentRepo = CourseAssignments().AsMockRepository();
            mockModelBindingHelperAdaptor = new Mock<IModelBindingHelperAdaptor>();

            var mockUnitOfWork = new Mock<UnitOfWork<ApplicationContext>>();
            mockUnitOfWork.Setup(c => c.DepartmentRepository).Returns(mockDepartmentRepo.Object);
            mockUnitOfWork.Setup(c => c.InstructorRepository).Returns(mockInstructorRepo.Object);
            mockUnitOfWork.Setup(c => c.CourseRepository).Returns(mockCourseRepo.Object);
            mockUnitOfWork.Setup(c => c.CourseAssignmentRepository).Returns(mockCourseAssignmentRepo.Object);

            sut = new InstructorsController(mockUnitOfWork.Object, mockModelBindingHelperAdaptor.Object);
        }

        [Theory]
        [InlineData(null, null)]
        public async Task Index_ReturnsAViewResult_WithAListOfInstructors(int? id, int? courseID)
        {
            var result = await sut.Index(id, courseID);

            var model = (InstructorIndexData)((ViewResult)result).Model;
            Assert.Equal(5, model.Instructors.Count());
            Assert.Null(model.Courses);
            Assert.Null(model.Enrollments);
        }

        [Theory]
        [InlineData(1, 6)]
        public async Task Index_ReturnsAViewResult_WithAListOfInstructorsAndCoursesEnrollments(int? id, int? courseID)
        {
            var result = await sut.Index(id, courseID);

            var model = (InstructorIndexData)((ViewResult)result).Model;
            Assert.Equal(5, model.Instructors.Count());
            Assert.Equal(id.Value, ((ViewResult)result).ViewData["InstructorID"]);
            Assert.Equal(courseID.Value, ((ViewResult)result).ViewData["CourseID"]);
            Assert.Equal(2, model.Courses.Count());
            Assert.Equal(2, model.Enrollments.Count());
        }

        [Theory]
        [InlineData(1,"Abercrombie")]
        public async Task Details_ReturnsAViewResult_WithInstructorModel(int id, string instructorName)
        {
            var result = await sut.Details(id);

            var model = (Instructor)((ViewResult)result).Model;

            Assert.Equal(instructorName, model.LastName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Details_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Details(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Fact]
        public void Create_ReturnsAViewResult_WithCoursesInViewData()
        {
            var result = (ViewResult)sut.Create();

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.Single(viewData);
            Assert.Equal("Courses", viewData.FirstOrDefault().Key);
        }

        [Fact]
        public async Task CreatePost_ReturnsRedirectToActionResult_Index()
        {
            var result = await sut.Create(new Instructor { }, null);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task CreatePost_ReturnsAViewResult_WithInvalidModel()
        {
            var instructor = Instructors().First();
            sut.ModelState.AddModelError("myerror", "error message");

            var result = await sut.Create(instructor, null);

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;
            Assert.Equal(instructor.LastName, ((Instructor)model).LastName);

            var modelState = ((ViewResult)result).ViewData.ModelState;
            Assert.Contains("myerror", modelState.Keys);
        }

        [Fact]
        public async Task CreatePost_ReturnsAViewResult_WithAddedCourses()
        {
            var instructor = Instructors().First();
            var instructorCourses = instructor.CourseAssignments.Count;
            var selectedCourses = instructor.CourseAssignments.Select(s => s.CourseID.ToString())
                .Append("1")
                .ToArray();
            sut.ModelState.AddModelError("myerror", "error message");

            var result = await sut.Create(instructor, selectedCourses);

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;
            Assert.Equal(instructor.LastName, ((Instructor)model).LastName);

            var modelState = ((ViewResult)result).ViewData.ModelState;
            Assert.Contains("myerror", modelState.Keys);

            Assert.Equal(instructorCourses+1, ((Instructor)model).CourseAssignments.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Edit_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Edit(id);

            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(1, "Abercrombie")]
        public async Task Edit_ReturnsAViewResult_WithInstructorModel(int id, string lastName)
        {
            var result = await sut.Edit(id);

            Assert.IsType<ViewResult>(result);

            Instructor model = (Instructor)((ViewResult)result).Model;
            Assert.Equal(lastName, model.LastName);
        }

        [Fact]
        public async Task EditPost_ReturnsANotFoundResult()
        {
            var result = await sut.Edit(null, null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditPost_ReturnsAVRedirectToAction_Index()
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<Controller>(), It.IsAny<Instructor>(), It.IsAny<string>(), It.IsAny<Expression<Func<Instructor, object>>[]>()))
                .Returns(Task.FromResult(true));

            var result = await sut.Edit(1, null);

            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsAViewResult_WithModelError_DbUpdateException()
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<Controller>(), It.IsAny<Instructor>(), It.IsAny<string>(), It.IsAny<Expression<Func<Instructor, object>>[]>()))
                .Returns(Task.FromResult(true));
            mockInstructorRepo.Setup(m => m.SaveChangesAsync())
                .Callback(() => throw new DbUpdateException("myexception", new Exception()));

            var result = await sut.Edit(1, null);

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.Count > 0);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Delete_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Delete(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(1, "Abercrombie")]
        public async Task Delete_ReturnsAViewAction_WithInstructorModel(int id, string lastName)
        {
            var result = await sut.Delete(id);

            Assert.IsType<ViewResult>(result);

            var model = (Instructor)((ViewResult)result).Model;
            Assert.Equal(lastName, model.LastName);
        }

        [Theory]
        [InlineData(5)]
        public async Task Delete_ReturnsARedirectToAction_Index(int id)
        {
            var result = await sut.DeleteConfirmed(id);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

        private List<Instructor> Instructors()
        {
            return new List<Instructor>
            {
                new Instructor { ID = 1, FirstMidName = "Kim",     LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11"),
                    CourseAssignments = new List<CourseAssignment>
                    {
                        new CourseAssignment {
                            CourseID = Courses().Single(c => c.Title == "Composition" ).ID,
                            InstructorID = 1,
                            Course = new Course {
                                ID = 6, CourseNumber = 2021, Title = "Composition", Credits = 3, DepartmentID = Departments().Single( s => s.Name == "English").ID,
                                Enrollments = new List<Enrollment>
                                {
                                    new Enrollment {
                                        StudentID = Students().Single(s => s.LastName == "Li").ID,
                                        CourseID = Courses().Single(c => c.Title == "Composition").ID,
                                        Grade = Grade.B
                                    },
                                    new Enrollment {
                                        StudentID = Students().Single(s => s.LastName == "Alonso").ID,
                                        CourseID = Courses().Single(c => c.Title == "Composition" ).ID,
                                        Grade = Grade.B
                                    },
                                }
                            },
                        },
                        new CourseAssignment {
                            CourseID = Courses().Single(c => c.Title == "Literature" ).ID,
                            InstructorID = 1,
                            Course = new Course {
                                ID = 7, CourseNumber = 2042, Title = "Literature", Credits = 4, DepartmentID = Departments().Single( s => s.Name == "English").ID,
                                Enrollments = new List<Enrollment>
                                {
                                    new Enrollment {
                                        StudentID = Students().Single(s => s.LastName == "Justice").ID,
                                        CourseID = Courses().Single(c => c.Title == "Literature").ID,
                                        Grade = Grade.B
                                    }
                                }
                            }
                        }
                    },

                },
                new Instructor { ID = 2, FirstMidName = "Fadi",    LastName = "Fakhouri", HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { ID = 3 ,FirstMidName = "Roger",   LastName = "Harui", HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { ID = 4, FirstMidName = "Candace", LastName = "Kapoor", HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { ID = 5, FirstMidName = "Roger",   LastName = "Zheng", HireDate = DateTime.Parse("2004-02-12") }
            };
        }
        private List<Department> Departments()
        {
            return new List<Department>
            {
                    new Department { ID = 1, Name = "English",     Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 1 },
                    new Department { ID = 2, Name = "Mathematics", Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 2 },
                    new Department { ID = 3, Name = "Engineering", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 3 },
                    new Department { ID = 4, Name = "Economics",   Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 4 }
            };
        }

        private List<Course> Courses()
        {
            var departments = Departments();
            return new List<Course>
            {
                new Course {ID = 1, CourseNumber = 1050, Title = "Chemistry", Credits = 3, DepartmentID = departments.Single( s => s.Name == "Engineering").ID },
                new Course {ID = 2, CourseNumber = 4022, Title = "Microeconomics", Credits = 3, DepartmentID = departments.Single( s => s.Name == "Economics").ID },
                new Course {ID = 3, CourseNumber = 4041, Title = "Macroeconomics", Credits = 3, DepartmentID = departments.Single( s => s.Name == "Economics").ID },
                new Course {ID = 4, CourseNumber = 1045, Title = "Calculus", Credits = 4, DepartmentID = departments.Single( s => s.Name == "Mathematics").ID },
                new Course {ID = 5, CourseNumber = 3141, Title = "Trigonometry", Credits = 4, DepartmentID = departments.Single( s => s.Name == "Mathematics").ID },
                new Course {ID = 6, CourseNumber = 2021, Title = "Composition", Credits = 3, DepartmentID = departments.Single( s => s.Name == "English").ID },
                new Course {ID = 7, CourseNumber = 2042, Title = "Literature", Credits = 4, DepartmentID = departments.Single( s => s.Name == "English").ID }
            };

        }

        private List<CourseAssignment> CourseAssignments()
        {
            var courses = Courses();
            var instructors = Instructors();
            return new List<CourseAssignment>
            {
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).ID,
                    InstructorID = instructors.FirstOrDefault(i => i.LastName == "Kapoor").ID
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
        }

        private List<Student> Students()
        {
            return new List<Student>
                {
                    new Student{ ID = 1, FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")},
                    new Student{ ID = 2, FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01")},
                    new Student{ ID = 3, FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2003-09-01")},
                    new Student{ ID = 4, FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2002-09-01")},
                    new Student{ ID = 5, FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2002-09-01")},
                    new Student{ ID = 6, FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2001-09-01")},
                    new Student{ ID = 7, FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2003-09-01")},
                    new Student{ ID = 8, FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2005-09-01")}
            };
        }
    }
}
