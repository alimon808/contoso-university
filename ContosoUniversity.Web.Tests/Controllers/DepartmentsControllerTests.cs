using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ContosoUniversity.Tests;
using ContosoUniversity.Common.Interfaces;
using AutoMapper;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class DepartmentsControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IRepository<Department>> mockDepartmentRepo;
        private readonly Mock<IPersonRepository<Instructor>> mockInstructorRepo;
        private readonly Mock<IModelBindingHelperAdaptor> mockModelBindingHelperAdaptor;
        private readonly IMapper _mapper;
        DepartmentsController sut;

        public DepartmentsControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockDepartmentRepo = Departments.AsMockRepository();
            mockInstructorRepo = Instructors.AsMockPersonRepository();
            mockModelBindingHelperAdaptor = new Mock<IModelBindingHelperAdaptor>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<WebProfile>();
            });
            _mapper = config.CreateMapper();


            var mockUnitOfWork = new Mock<UnitOfWork<ApplicationContext>>();
            mockUnitOfWork.Setup(c => c.DepartmentRepository).Returns(mockDepartmentRepo.Object);
            mockUnitOfWork.Setup(c => c.InstructorRepository).Returns(mockInstructorRepo.Object);

            sut = new DepartmentsController(mockUnitOfWork.Object, mockModelBindingHelperAdaptor.Object, _mapper);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfDepartments()
        {
            var result = await sut.Index();
            var model = (List<DepartmentDetailsViewModel>)((ViewResult)result).Model;

            Assert.Equal(4, model.Count);
        }

        [Theory]
        [InlineData(3, "Engineering")]
        public async Task Details_ReturnsAViewResult_WithDepartmentModel(int id, string departmentName)
        {
            var result = await sut.Details(id);
            var model = (DepartmentDetailsViewModel)((ViewResult)result).Model;

            Assert.Equal(departmentName, model.Name);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Details_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Details(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }


        [Fact]
        public void Create_ReturnsAViewResult_WithInstructorIdInViewData()
        {
            var result = (ViewResult)sut.Create();

            Assert.IsType<ViewResult>(result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.Single(viewData);
            Assert.Equal("InstructorID", viewData.FirstOrDefault().Key);
        }

        [Fact]
        public async Task CreatePost_ReturnsRedirectToActionResult_Index()
        {
            var vm = new DepartmentCreateViewModel
            {
                Name = "Biology",
                Budget = 100,
                StartDate = DateTime.Parse("2007-09-01"),
                InstructorID = 1
            };

            var result = await sut.Create(vm);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task CreatePost_ReturnsAViewResult_WithInvalidModel()
        {
            sut.ModelState.AddModelError("myerror", "error message");

            var result = await sut.Create(new DepartmentCreateViewModel { });

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("myerror"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(null)]
        public async Task Edit_ReturnsANotFoundResult(int? id)
        {
            var result = await sut.Edit(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(1, "English")]
        public async Task Edit_ReturnsAViewResult_WithDepartmentModel(int id, string departmentName)
        {
            var result = await sut.Edit(id);

            Assert.IsType<ViewResult>(result);

            var vm = (DepartmentEditViewModel)((ViewResult)result).Model;
            Assert.Equal(departmentName, vm.Name);
        }

        [Theory]
        [InlineData(0)]
        public async Task EditPost_ReturnsAViewResult_WithDeletedDepartmentModelError(int id)
        {
            DepartmentEditViewModel vm = new DepartmentEditViewModel { ID = id };
            var result = await sut.Edit(vm);

            Assert.IsType<ViewResult>(result);
            var model = (DepartmentEditViewModel)((ViewResult)result).Model;
            Assert.Null(model.Name);
            Assert.True(((ViewResult)result).ViewData.ModelState.Count > 0);
            Assert.True(((ViewResult)result).ViewData.ContainsKey("InstructorID"));

        }

        [Fact]
        public async Task EditPost_ReturnsRedirectToActionResult_Index()
        {
            //mockModelBindingHelperAdaptor.Setup(m => m.TryUpdateModelAsync(It.IsAny<Controller>(), It.IsAny<Department>(), It.IsAny<string>(), It.IsAny<Expression<Func<Department, object>>[]>()))
            //    .Returns(Task.FromResult(true));
            
            var vm = new DepartmentEditViewModel { ID = 1 };

            var result = await sut.Edit(vm);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(null, null)]
        public async Task Delete_ReturnsANotFoundResult(int? id, bool? concurrencyError)
        {
            var result = await sut.Delete(id, concurrencyError);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(1, true, "English")]
        public async Task Delete_ReturnsAViewAction_WithDepartmentModel_AndConcurrencyErrorMessage(int? id, bool? concurrencyError, string departmentName)
        {
            var result = await sut.Delete(id, concurrencyError);

            Assert.IsType<ViewResult>(result);

            var model = (Department)((ViewResult)result).Model;
            Assert.Equal(departmentName, model.Name);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("ConcurrencyErrorMessage"));
        }

        [Theory]
        [InlineData(0, true)]
        public async Task Delete_ReturnsARedirectToAction_Index(int? id, bool? concurrencyError)
        {
            var result = await sut.Delete(id, concurrencyError);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task DeletePost_ReturnsARedirectToActionResult_Index(int id)
        {
            var result = await sut.Delete(id);

            Assert.IsType<RedirectToActionResult>(result);
            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

        private List<Instructor> Instructors { get; } = new List<Instructor>
        {
                new Instructor { ID = 1, FirstMidName = "Kim",     LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { ID = 2, FirstMidName = "Fadi",    LastName = "Fakhouri", HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { ID = 3 ,FirstMidName = "Roger",   LastName = "Harui", HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { ID = 4, FirstMidName = "Candace", LastName = "Kapoor", HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { ID = 5, FirstMidName = "Roger",   LastName = "Zheng", HireDate = DateTime.Parse("2004-02-12") }
        };

        private List<Department> Departments { get; } = new List<Department>
        {
                new Department { ID = 1, Name = "English",     Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 1 },
                new Department { ID = 2, Name = "Mathematics", Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 2 },
                new Department { ID = 3, Name = "Engineering", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 3 },
                new Department { ID = 4, Name = "Economics",   Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 4 }
        };
        
    }
}
