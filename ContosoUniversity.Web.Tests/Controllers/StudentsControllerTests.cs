using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class StudentsControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IPersonRepository<Student>> mockStudentRepo;
        StudentsController sut;
        private readonly Mock<IModelBindingHelperAdaptor> mockModelBindingHelperAdaptor;

        public StudentsControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockStudentRepo = Students().AsMockPersonRepository();

            var mockUnitOfWork = new Mock<UnitOfWork<ApplicationContext>>();
            mockUnitOfWork.Setup(c => c.StudentRepository).Returns(mockStudentRepo.Object);

            mockModelBindingHelperAdaptor = new Mock<IModelBindingHelperAdaptor>();
            sut = new StudentsController(mockUnitOfWork.Object, mockModelBindingHelperAdaptor.Object);
        }

        [Theory]
        [InlineData(null, null, null, null)]
        public async Task Index_ReturnsAViewResult_WithAListOfPaginatedListOfStudents(string sortOrder, string searchString, int? page, string currentFilter)
        {
            var result = await sut.Index(sortOrder, searchString, page, currentFilter);

            Assert.IsType<ViewResult>(result);

            var model = ((ViewResult)result).Model;
            Assert.IsType<PaginatedList<Student>>(model);

            var viewData = ((ViewResult)result).ViewData;
            Assert.Null(viewData["CurrentSort"]);
        }

        [Theory]
        [InlineData(1, "Alexander")]
        public async Task Details_ReturnsAViewResult_WithStudentModel(int id, string lastName)
        {
            var result = await sut.Details(id);

            var model = (Student)((ViewResult)result).Model;
            Assert.Equal(lastName, model.LastName);
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
            var result = sut.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreatePost_ReturnsRedirectToActionResult_Index()
        {
            var result = await sut.Create(new Student());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task CreatePost_ReturnsAViewResult_WithInvalidModel()
        {
            var student = new Student { ID = 9, FirstMidName = "John", LastName = "Wick", EnrollmentDate = DateTime.Parse("2005-09-01") };
            sut.ModelState.AddModelError("myerror", "my error message");

            var result = await sut.Create(student);

            Assert.IsType<ViewResult>(result);

            var model = (Student)((ViewResult)result).Model;
            Assert.Equal("Wick", model.LastName);
        }

        [Fact]
        public async Task CreatePost_ReturnsAViewResult_WithInvalidModel_DbUpdateException()
        {
            var student = new Student { ID = 9, FirstMidName = "John", LastName = "Wick", EnrollmentDate = DateTime.Parse("2005-09-01") };
            mockStudentRepo.Setup(m => m.SaveChangesAsync())
                .Callback(() => throw new DbUpdateException("myexception", new Exception()))
                .Returns(Task.FromResult(0));

            var result = await sut.Create(student);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ModelState.Count > 0);
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
        [InlineData(1, "Alexander")]
        public async Task Edit_ReturnsAViewResult_WithStudentModel(int id, string lastName)
        {
            var result = await sut.Edit(id);

            Assert.IsType<ViewResult>(result);

            var model = (Student)((ViewResult)result).Model;
            Assert.Equal(lastName, model.LastName);
        }

        [Fact]
        public async Task EditPost_ReturnsAViewResult_WithInvalidModel()
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<ControllerBase>(), It.IsAny<Student>(), It.IsAny<string>(), It.IsAny<Expression<Func<Student, object>>[]>()))
                .Callback(() => sut.ModelState.AddModelError("mymodelerror", "my error message"))
                .Returns(Task.FromResult(false));

            var result = await sut.EditPost(1);

            Assert.IsType<ViewResult>(result);

            Assert.True( ((ViewResult)result).ViewData.ModelState.ContainsKey("mymodelerror"));
        }

        [Fact]
        public async Task EditPost_ReturnsAViewResult_WithInvalidModel_DbUpdateException()
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<ControllerBase>(), It.IsAny<Student>(), It.IsAny<string>(), It.IsAny<Expression<Func<Student, object>>[]>()))
                .Returns(Task.FromResult(true));

            mockStudentRepo.Setup(m => m.SaveChangesAsync())
                .Callback(() => throw new DbUpdateException("myexception", new Exception()))
                .Returns(Task.FromResult(0));

            var result = await sut.EditPost(1);

            Assert.True(((ViewResult)result).ViewData.ModelState.Count > 0);
        }

        [Fact]
        public async Task EditPost_ReturnsAVRedirectToAction_Index()
        {
            mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<ControllerBase>(), It.IsAny<Student>(), It.IsAny<string>(), It.IsAny<Expression<Func<Student, object>>[]>()))
                .Returns(Task.FromResult(true));

            var result = await sut.EditPost(1);

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

        [Fact]
        public async Task Delete_ReturnsAViewResult_WithErrorMessageInViewData()
        {
            bool saveChangesError = true;

            var result = await sut.Delete(1, saveChangesError);

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("ErrorMessage"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public async Task Delete_ReturnsARedirectToAction_Index(int? id)
        {
            var result = await sut.DeleteConfirmed(id.Value);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectToAction_DeleteAction()
        {
            mockStudentRepo.Setup(m => m.SaveChangesAsync())
                .Callback(() => throw new DbUpdateException("myexception", new Exception()))
                .Returns(Task.FromResult(0));

            var result = await sut.DeleteConfirmed(1);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Delete", actionName);
            Assert.True(((RedirectToActionResult)result).RouteValues.TryGetValue("id", out object id));
            Assert.True(((RedirectToActionResult)result).RouteValues.TryGetValue("saveChangesError", out object saveChangesError));
            Assert.Equal(1, id);
            Assert.True((bool)saveChangesError);
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
