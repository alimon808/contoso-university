using ContosoUniversity.Controllers;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests.Controllers
{
    public class CoursesControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IRepository<Course>> mockCourseRepo;
        private readonly Mock<IRepository<Department>> mockDepartmentRepo;
        private readonly CoursesController sut;

        public CoursesControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockCourseRepo = Courses().AsMockRepository();
            mockDepartmentRepo = Departments().AsMockRepository();
            sut = new CoursesController(mockCourseRepo.Object, mockDepartmentRepo.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfOfCourses()
        {
            var result = await sut.Index();

            Assert.IsType(typeof(ViewResult), result);
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

        //[Theory]
        //[InlineData(0)]
        //[InlineData(null)]
        //public void Details_ReturnsANotFoundResult(long? id)
        //{
        //    var result = sut.Details(id).Result;

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(NotFoundResult), result);
        //    Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        //}

        //[Fact]
        //public void Create_ReturnsAViewResult()
        //{
        //    var result = (ViewResult)sut.Create();

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);

        //    var viewData = ((ViewResult)result).ViewData;
        //    Assert.NotNull(viewData);
        //    Assert.True(viewData.ContainsKey("DepartmentID"));
        //}

        //[Fact]
        //public async Task CreatePost_ReturnsRedirectToActionResult_Index()
        //{
        //    CourseCreateViewModel vm = new CourseCreateViewModel { Number = 9999, Credits = 4, DepartmentID = 1, Title = "Algebra 2" };

        //    var result = await sut.Create(vm);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(RedirectToActionResult), result);
        //    Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);

        //    var course = mockCourseRepo.Object.GetAll().Where(c => c.Number == vm.Number).FirstOrDefault();
        //    Assert.NotNull(course);
        //    Assert.Equal(vm.Title, course.Title);
        //}

        //[Fact]
        //public async Task CreatePost_ReturnsAViewResult_WithInvalidCourseCreateViewModel()
        //{
        //    CourseCreateViewModel vm = new CourseCreateViewModel { Number = 9999, Credits = 4, DepartmentID = 1, Title = "Algebra 2" };
        //    sut.ModelState.AddModelError("ErrorMessage", "error message");

        //    var result = await sut.Create(vm);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);

        //    var viewData = ((ViewResult)result).ViewData;
        //    CourseCreateViewModel model = (CourseCreateViewModel)viewData.Model;
        //    Assert.Equal(vm.Number, model.Number);
        //    Assert.True(viewData.ContainsKey("DepartmentID"));
        //    Assert.True(viewData.ModelState.ContainsKey("ErrorMessage"));
        //}

        //[Theory]
        //[InlineData(0)]
        //[InlineData(null)]
        //public async Task Edit_ReturnsANotFoundResult(long? id)
        //{
        //    var result = await sut.Edit(id);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(NotFoundResult), result);
        //    Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        //}

        //[Theory]
        //[InlineData(3, "Macroeconomics", 4041, 3, 4)]
        //public async Task Edit_ReturnsAViewResult_WithCourseEditViewModel(int id, string title, int number, int credits, long departmentId)
        //{
        //    var result = await sut.Edit(id);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);

        //    var viewData = ((ViewResult)result).ViewData;
        //    CourseEditViewModel model = (CourseEditViewModel)viewData.Model;
        //    Assert.Equal(title, model.Title);
        //    Assert.Equal(number, model.Number);
        //    Assert.Equal(departmentId, model.DepartmentID);
        //    Assert.True(viewData.ContainsKey("DepartmentID"));
        //}

        //[Theory]
        //[InlineData(null)]
        //public async Task EditPost_ReturnsANotFoundResult(long? id)
        //{
        //    CourseEditViewModel vm = new CourseEditViewModel
        //    {
        //        Id = id
        //    };

        //    var result = await sut.EditPost(vm);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(NotFoundResult), result);
        //    Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        //}

        //[Theory]
        //[InlineData(0)]
        //public async Task EditPost_ReturnsAViweResult_WithCourseDoesNotExist(long? id)
        //{
        //    CourseEditViewModel vm = new CourseEditViewModel
        //    {
        //        Id = id
        //    };

        //    var result = await sut.EditPost(vm);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);

        //    var model = ((ViewResult)result).Model;
        //    Assert.IsType(typeof(CourseEditViewModel), model);

        //    var modelState = ((ViewResult)result).ViewData.ModelState;
        //    Assert.Equal(1,modelState.Count);
        //}

        //[Fact]
        //public async Task EditPost_ReturnsAViewResult_WithValidCourse_ThrowsDbUpdateException()
        //{
        //    CourseEditViewModel vm = new CourseEditViewModel
        //    {
        //        Id = 1
        //    };

        //    mockCourseRepo.Setup(m => m.UpdateAsync(It.IsAny<Course>(), It.IsAny<byte[]>()))
        //        .Throws(new DbUpdateException("myexception", new Exception()));

        //    var result = await sut.EditPost(vm);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);

        //    var viewData = ((ViewResult)result).ViewData;
        //    var model = viewData.Model;
        //    Assert.IsType(typeof(CourseEditViewModel), model);
        //    Assert.True(viewData.ContainsKey("DepartmentID"));
        //    Assert.True(viewData.ModelState.ContainsKey("UpdateException"));
        //}


        //[Fact]
        //public async Task EditPost_ReturnsAVRedirectToAction_Index()
        //{
        //    CourseEditViewModel vm = new CourseEditViewModel
        //    {
        //        Id = 1,
        //        DepartmentID = 1,
        //        Credits = 5,
        //        Title = "Chemistry - update"
        //    };

        //    var result = await sut.EditPost(vm);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(RedirectToActionResult), result);
        //    Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        //}

        //[Theory]
        //[InlineData(0)]
        //[InlineData(null)]
        //public async Task Delete_ReturnsANotFoundResult(long? id)
        //{
        //    var result = await sut.Delete(id);

        //    Assert.NotNull(result);
        //    Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        //}

        //[Theory]
        //[InlineData(1, "Chemistry")]
        //public async Task Delete_ReturnsAViewResult_WithCourse(long id, string title)
        //{
        //    var result = await sut.Delete(id);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);

        //    var viewData = ((ViewResult)result).ViewData;
        //    Course model = (Course)viewData.Model;
        //    Assert.Equal(title, model.Title);

        //}

        //[Theory]
        //[InlineData(null)]
        //[InlineData(0)]
        //public async Task DeletePost_ReturnsARedirectToAction_Delete(long? id)
        //{
        //    var result = await sut.DeleteConfirmed(id);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(RedirectToActionResult), result);
        //    var actionName = ((RedirectToActionResult)result).ActionName;
        //    Assert.Equal("Delete", actionName);
        //}

        //[Theory]
        //[InlineData(5)]
        //public async Task DeletePost_ReturnsARedirectToAction_Index(long? id)
        //{
        //    var preCount = mockCourseRepo.Object.GetAll().Count();
        //    var result = await sut.DeleteConfirmed(id.Value);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(RedirectToActionResult), result);
        //    var actionName = ((RedirectToActionResult)result).ActionName;
        //    Assert.Equal("Index", actionName);
        //    Assert.Equal(preCount - 1, mockCourseRepo.Object.GetAll().Count());
        //}

        //[Fact]
        //public async Task UpdateCourseCredits_ReturnsAViewResult()
        //{
        //    var result = await sut.UpdateCourseCredits(0);

        //    Assert.NotNull(result);
        //    Assert.IsType(typeof(ViewResult), result);
        //}

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
