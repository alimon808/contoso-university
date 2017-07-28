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

namespace ContosoUniversity.UnitTests.Web
{
    public class StudentsControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<IPersonRepository<Student>> mockStudentRepo;
        StudentsController sut;

        public StudentsControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockStudentRepo = Students().AsMockPersonRepository();
            sut = new StudentsController(mockStudentRepo.Object);
        }

        [Theory]
        [InlineData(null, null, null, null)]
        public async Task Index_ReturnsAViewResult_WithAListOfPaginatedListOfStudents(string sortOrder, string searchString, int? page, string currentFilter)
        {
            var result = await sut.Index(sortOrder, searchString, page, currentFilter);

            Assert.IsType(typeof(ViewResult), result);

            var model = ((ViewResult)result).Model;
            Assert.IsType(typeof(PaginatedList<Student>), model);

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

            Assert.IsType(typeof(NotFoundResult), result);
            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
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
