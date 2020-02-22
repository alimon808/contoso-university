using Xunit;
using ContosoUniversity.Api.Controllers;
using Moq;
using ContosoUniversity.Data.Entities;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ContosoUniversity.Tests;
using ContosoUniversity.Common.Interfaces;
using AutoMapper;
using ContosoUniversity.Common.DTO;
using ContosoUniversity.Api.DTO;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity.Api.Tests
{
    public class DepartmentsApiControllerTests
    {
        private readonly DepartmentsController _sut;
        private readonly Mock<IRepository<Department>> _mockDepartmentRepo;
        private readonly Mock<IPersonRepository<Instructor>> _mockInstructorRepo;
        private readonly IMapper _mapper;
        public DepartmentsApiControllerTests()
        {
            _mockDepartmentRepo = Departments.AsMockRepository<Department>();
            _mockInstructorRepo = Instructors.AsMockPersonRepository<Instructor>();

            var mockUnitOfWork = new Mock<UnitOfWork<ApiContext>>();
            mockUnitOfWork.Setup(c => c.DepartmentRepository).Returns(_mockDepartmentRepo.Object);
            mockUnitOfWork.Setup(c => c.InstructorRepository).Returns(_mockInstructorRepo.Object);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApiProfile>();
            });
            _mapper = config.CreateMapper();
            _sut = new DepartmentsController(mockUnitOfWork.Object, _mapper);
        }

        [Fact]
        public void HttpGet_ReturnsAListOfDepartments()
        {
            var result = _sut.GetAll();

            Assert.IsType<List<DepartmentDTO>>(result);
            Assert.Equal(4, ((List<DepartmentDTO>)result).Count);
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

            Assert.IsType<ObjectResult>(result);
            var dto = (DepartmentDTO)((ObjectResult)result).Value;
            Assert.Equal(departmentName, dto.Name);
            Assert.Equal(budget, dto.Budget);
        }

        [Fact]
        public async Task HttpPost_ReturnsCreatedAtRouteResult_WithDepartmentEntity()
        {
            var department = new Department { ID = 5, Name = "Physics", Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1 };

            var departmentToAdd = _mapper.Map<CreateDepartmentDTO>(department);
            var result = await _sut.Create(departmentToAdd);

            Assert.NotNull(result);
            Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(201, ((CreatedAtRouteResult)result).StatusCode);

            var departmentCreated = _mapper.Map<DepartmentDTO>(((CreatedAtRouteResult)result).Value);
            Assert.Equal(department.ID, departmentCreated.ID);
            Assert.Equal(departmentToAdd.Name, department.Name);
        }

        [Fact]
        public async Task HttpPost_ReturnsABadRequestResult_WhenInstructorIdDoesNotExist()
        {
            var department = new Department
            {
                ID = 5,
                Name = "Physics",
                Budget = 100000,
                AddedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                StartDate = DateTime.Parse("2007-09-01"),
                InstructorID = 99
            };
            var departmentToAdd = _mapper.Map<CreateDepartmentDTO>(department);
            var result = await _sut.Create(departmentToAdd);

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, ((BadRequestObjectResult)result).StatusCode);

            var value = ((BadRequestObjectResult)result).Value as SerializableError;
            Assert.True(value.Count == 1);
            Assert.True(value.ContainsKey("InstructorID"));
        }

        [Fact]
        public async Task HttpPut_ReturnsABadRequestResult_WithNullDepartment()
        {
            var result = await _sut.Update(1, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task HttpPut_ReturnsABadRequestResult_WithInvalidDepartmentID()
        {
            var departmentToUpdate = new Department { ID = 1, Name = "English 2", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1, RowVersion = new byte[] { } };
            var dto = _mapper.Map<DepartmentDTO>(departmentToUpdate);
            var result = await _sut.Update(2, dto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task HttpPut_ReturnsANotFoundResult()
        {
            var departmentToUpdate = new Department { ID = 0, Name = "English 2", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1, RowVersion = new byte[] { } };
            var dto = _mapper.Map<DepartmentDTO>(departmentToUpdate);

            var result = await _sut.Update(departmentToUpdate.ID, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task HttpPut_ReturnsANoContentResult()
        {
            var departmentToUpdate = new Department { ID = 1, Name = "English 2", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 1, RowVersion = new byte[] { } };
            var dto = _mapper.Map<DepartmentDTO>(departmentToUpdate);
            var result = await _sut.Update(departmentToUpdate.ID, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task HttpDelete_ReturnsNoContentResult()
        {
            var result = await _sut.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task HttpDelete_ReturnsNotFoundResult()
        {
            var result = await _sut.Delete(0);

            Assert.IsType<NotFoundResult>(result);
        }

        private List<Department> Departments { get; } = new List<Department>
        {
            new Department { ID = 1, Name = "English",     Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 1 },
            new Department { ID = 2, Name = "Mathematics", Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 2 },
            new Department { ID = 3, Name = "Engineering", Budget = 350000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 3 },
            new Department { ID = 4, Name = "Economics",   Budget = 100000, AddedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, StartDate = DateTime.Parse("2007-09-01"), InstructorID  = 4 }
        };

        private List<Instructor> Instructors { get; } = new List<Instructor>
        {
            new Instructor {ID = 1, HireDate = DateTime.Now, FirstMidName = "Albert", LastName = "Einstein" }
        };
    }
}
