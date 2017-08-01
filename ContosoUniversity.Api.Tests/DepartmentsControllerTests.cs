using Xunit;
using ContosoUniversity.Api.Controllers;
using Moq;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContosoUniversity.Api.Tests
{
    public class DepartmentsApiControllerTests
    {
        private readonly DepartmentsController _sut;
        private readonly Mock<IRepository<Department>> _mockDepartmentRepo;

        public DepartmentsApiControllerTests()
        {
            _mockDepartmentRepo = Departments.AsMockRepository<Department>();
            _sut = new DepartmentsController(_mockDepartmentRepo.Object);
        }

        [Fact]
        public void HttpGet_ReturnsAListOfDepartments()
        {
            var result = _sut.GetAll();

            Assert.IsType(typeof(List<Department>), result);
            Assert.Equal(4, ((List<Department>)result).Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public void HttpGet_ReturnsANotFoundResult(int id)
        {
            var result = _sut.GetById(id);

            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Theory]
        [InlineData(1, "English", 350000)]
        public void HttpGet_ReturnsADepartmentEntity(int id, string departmentName, int budget)
        {
            var result = _sut.GetById(id);

            Assert.IsType(typeof(ObjectResult), result);
            var department = (Department)((ObjectResult)result).Value;
            Assert.Equal(departmentName, department.Name);
            Assert.Equal(budget, department.Budget);
        }

        [Fact]
        public async Task HttpPost_ReturnsCreatedAtRouteResult_WithDepartmentEntity()
        {
            var departmentToAdd = new Department { ID = 5, Name = "Physics", Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 4 };
            var result = await _sut.Create(departmentToAdd);

            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(201, ((CreatedAtRouteResult)result).StatusCode);

            var department = (Department)((CreatedAtRouteResult)result).Value;
            Assert.Equal(departmentToAdd.ID, department.ID);
            Assert.Equal(departmentToAdd.Name, department.Name);
        }

        [Fact]
        public async Task HttpPost_ReturnsABadRequestResult()
        {
            var result = await _sut.Create(null);

            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
            Assert.Equal(400, ((BadRequestResult)result).StatusCode);
        }

        [Fact]
        public async Task HttpPut_ReturnsABadRequestResult_WithNullDepartment()
        {
            var result = await _sut.Update(1, null);

            Assert.IsType(typeof(BadRequestResult), result);
        }

        [Fact]
        public async Task HttpPut_ReturnsABadRequestResult_WithInvalidDepartmentID()
        {
            var departmentToUpdate = new Department { ID = 1, Name = "English 2", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1, RowVersion = new byte[] { } };
            var result = await _sut.Update(2, departmentToUpdate);

            Assert.IsType(typeof(BadRequestResult), result);
        }

        [Fact]
        public async Task HttpPut_ReturnsANotFoundResult()
        {
            var departmentToUpdate = new Department { ID = 0, Name = "English 2", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1, RowVersion = new byte[] { } };

            var result = await _sut.Update(departmentToUpdate.ID, departmentToUpdate);

            Assert.IsType(typeof(NotFoundResult), result);
        }

        [Fact]
        public async Task HttpPut_ReturnsANoContentResult()
        {
            var departmentToUpdate = new Department { ID = 1, Name = "English 2", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1, RowVersion = new byte[] { } };

            var result = await _sut.Update(departmentToUpdate.ID, departmentToUpdate);

            Assert.IsType(typeof(NoContentResult), result);
        }

        [Fact]
        public async Task HttpDelete_ReturnsNoContentResult()
        {
            var result = await _sut.Delete(1);

            Assert.IsType(typeof(NoContentResult), result);
        }

        [Fact]
        public async Task HttpDelete_ReturnsNotFoundResult()
        {
            var result = await _sut.Delete(0);

            Assert.IsType(typeof(NotFoundResult), result);
        }

        private List<Department> Departments { get; } = new List<Department>
        {
            new Department { ID = 1, Name = "English",     Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 1 },
            new Department { ID = 2, Name = "Mathematics", Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 2 },
            new Department { ID = 3, Name = "Engineering", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 3 },
            new Department { ID = 4, Name = "Economics",   Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 4 }
        };
    }
}
