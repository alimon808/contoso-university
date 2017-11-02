using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Data.Entities;
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
    public class CoursesControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IRepository<Course>> mockCourseRepo;
        private readonly Mock<IRepository<Department>> mockDepartmentRepo;
        private readonly Mock<IModelBindingHelperAdaptor> mockModelBindingHelperAdaptor;
        private readonly CoursesController sut;

        public CoursesControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockCourseRepo = Courses().AsMockRepository();
            mockDepartmentRepo = Departments().AsMockRepository();
            mockModelBindingHelperAdaptor = new Mock<IModelBindingHelperAdaptor>();

            var mockUnitOfWork = new Mock<UnitOfWork<ApplicationContext>>();
            mockUnitOfWork.Setup(c => c.DepartmentRepository).Returns(mockDepartmentRepo.Object);
            mockUnitOfWork.Setup(c => c.CourseRepository).Returns(mockCourseRepo.Object);

            sut = new CoursesController(mockUnitOfWork.Object, mockModelBindingHelperAdaptor.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfOfCourses()
        {
            var result = await sut.Index();

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;
            Assert.Equal(Courses().Count, ((List<Course>)model).Count);
        }

        [Theory]
        [InlineData(3, "Macroeconomics")]
        public async Task Details_ReturnsAViewResult_WithCourseModel(int id, string title)
        {
            var result = await sut.Details(id);
            var model = (Course)((ViewResult)result).Model;

            Assert.NotNull(result);
            Assert.Equal(title, model.Title);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Details_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Details(id);

            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Fact]
        public void Create_ReturnsAViewResult()
        {
            var result = (ViewResult)sut.Create();

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("DepartmentID"));
        }

        [Fact]
        public async Task CreatePost_ReturnsRedirectToActionResult_Index()
        {
            Course courseToAdd = new Course { CourseNumber = 9999, Credits = 4, DepartmentID = 1, Title = "Algebra 2" };

            var result = await sut.Create(courseToAdd);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);

            var course = mockCourseRepo.Object.GetAll().Where(c => c.CourseNumber == courseToAdd.CourseNumber).FirstOrDefault();
            Assert.Equal(courseToAdd.Title, course.Title);
        }

        [Fact]
        public async Task CreatePost_ReturnsAViewResult_WithInvalidCourseCreateViewModel()
        {
            Course courseToAdd = new Course { CourseNumber = 9999, Credits = 4, DepartmentID = 1, Title = "Algebra 2" };
            sut.ModelState.AddModelError("MyErrorMessage", "error message");

            var result = await sut.Create(courseToAdd);

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Course model = (Course)viewData.Model;
            Assert.True(viewData.ContainsKey("DepartmentID"));
            Assert.True(viewData.ModelState.ContainsKey("MyErrorMessage"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Edit_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Edit(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(3, "Macroeconomics", 4041, 4)]
        public async Task Edit_ReturnsAViewResult_WithCourseEditViewModel(int id, string title, int number, long departmentId)
        {
            var result = await sut.Edit(id);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Course model = (Course)viewData.Model;
            Assert.Equal(title, model.Title);
            Assert.Equal(number, model.CourseNumber);
            Assert.Equal(departmentId, model.DepartmentID);
            Assert.True(viewData.ContainsKey("DepartmentID"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public async Task EditPost_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.EditPost(id);
            
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
            
        }

       [Theory]
       [InlineData(1, 1050, 3, 3, "Chemistry")]
        public async Task EditPost_ReturnsAViweResult_WithInvalidModel(int id, int courseNumber, int credits, int departmentID, string title)
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<Controller>(), It.IsAny<Course>(), It.IsAny<string>(), It.IsAny<Expression<Func<Course, object>>[]>()))
                .Callback(() => sut.ModelState.AddModelError("myerror", "testing error"))
                .Returns(Task.FromResult(false));

            var result = await sut.EditPost(id);

            Assert.IsType<ViewResult>(result);

            var model = ((ViewResult)result).Model as Course;
            Assert.Equal(id, model.ID);
            Assert.Equal(courseNumber, model.CourseNumber);
            Assert.Equal(credits, model.Credits);
            Assert.Equal(departmentID, model.DepartmentID);
            Assert.Equal(title, model.Title);

            var modelState = ((ViewResult)result).ViewData.ModelState;
            Assert.True(modelState.ContainsKey("myerror"));
        }

        [Theory]
        [InlineData(1)]
        public async Task EditPost_ReturnsAViewResult_WithValidCourse_ThrowsDbUpdateException(int id)
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<Controller>(), It.IsAny<Course>(), It.IsAny<string>(), It.IsAny<Expression<Func<Course, object>>[]>()))
                .Returns(Task.FromResult(true));
            mockCourseRepo.Setup(m => m.SaveChangesAsync())
                .Throws(new DbUpdateException("myDbUpdateException", new Exception()));

            var result = await sut.EditPost(id);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("DepartmentID"));
            Assert.Single(viewData.ModelState);
        }

        [Theory]
        [InlineData(1)]
        public async Task EditPost_ReturnsARedirectToAction_Index(int id)
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<Controller>(), It.IsAny<Course>(), It.IsAny<string>(), It.IsAny<Expression<Func<Course, object>>[]>()))
                .Returns(Task.FromResult(true));
            var result = await sut.EditPost(id);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
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
        [InlineData(1, "Chemistry")]
        public async Task Delete_ReturnsAViewResult_WithCourseModel(int id, string title)
        {
            var result = await sut.Delete(id);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Course model = (Course)viewData.Model;
            Assert.Equal(title, model.Title);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public async Task DeletePost_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.DeleteConfirmed(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(5)]
        public async Task DeletePost_ReturnsARedirectToAction_Index(int? id)
        {
            var result = await sut.DeleteConfirmed(id.Value);

            Assert.IsType<RedirectToActionResult>(result);
            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

        [Fact]
        public async Task UpdateCourseCredits_ReturnsAViewResult_WithRowsAffected()
        {
            var result = await sut.UpdateCourseCredits(2);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("RowsAffected"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task UpdateCourseCredits_ReturnsAViewResult_WithInvalidMultiplier(int? multiplier)
        {
            var result = await sut.UpdateCourseCredits(multiplier);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ModelState.ContainsKey("InvalidMultiplier"));
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
            return new List<Course>
            {
                new Course { ID = 1, CourseNumber = 1050, Title = "Chemistry", Credits = 3, DepartmentID = Departments().Single( s => s.Name == "Engineering").ID },
                new Course { ID = 2, CourseNumber = 4022, Title = "Microeconomics", Credits = 3, DepartmentID = Departments().Single( s => s.Name == "Economics").ID },
                new Course { ID = 3, CourseNumber = 4041, Title = "Macroeconomics", Credits = 3, DepartmentID = Departments().Single( s => s.Name == "Economics").ID },
                new Course { ID = 4, CourseNumber = 1045, Title = "Calculus", Credits = 4, DepartmentID = Departments().Single( s => s.Name == "Mathematics").ID },
                new Course { ID = 5, CourseNumber = 3141, Title = "Trigonometry", Credits = 4, DepartmentID = Departments().Single( s => s.Name == "Mathematics").ID },
                new Course { ID = 6, CourseNumber = 2021, Title = "Composition", Credits = 3, DepartmentID = Departments().Single( s => s.Name == "English").ID },
                new Course { ID = 7, CourseNumber = 2042, Title = "Literature", Credits = 4, DepartmentID = Departments().Single( s => s.Name == "English").ID }
            };
        }
    }
}
