using ContosoUniversity.Controllers;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.UnitTests.Controllers
{
    public class DepartmentsControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IRepository<Department>> mockDepartmentRepo;
        private readonly Mock<IPersonRepository<Instructor>> mockInstructorRepo;
        DepartmentsController sut;

        public DepartmentsControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockDepartmentRepo = Departments.AsMockRepository();
            mockInstructorRepo = Instructors.AsMockPersonRepository();
            sut = new DepartmentsController(mockDepartmentRepo.Object, mockInstructorRepo.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfDepartments()
        {
            var result = await sut.Index();
            var model = (List<Department>)((ViewResult)result).Model;

            Assert.Equal(4, model.Count);
        }

        [Theory]
        [InlineData(3, "Engineering")]
        public async Task Details_ReturnsAViewResult_WithDepartmentModel(int id, string departmentName)
        {
            var result = await sut.Details(id);
            var model = (Department)((ViewResult)result).Model;

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
