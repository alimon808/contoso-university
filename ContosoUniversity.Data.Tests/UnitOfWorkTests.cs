using ContosoUniversity.Common;
using ContosoUniversity.Common.Repositories;
using ContosoUniversity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.Data.Tests
{
    public class UnitOfWorkTests
    {
        private readonly UnitOfWork _sut;
        public UnitOfWorkTests()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseInMemoryDatabase("TestDb");

            var context = new ApplicationContext(builder.Options);
            _sut = new UnitOfWork(context);
        }

        [Fact]
        public void DepartmentRepository_ShouldReturnRepository()
        {

            var result = _sut.DepartmentRepository;

            Assert.IsType<Repository<Department>>(result);
        }

        [Fact]
        public void InstructorRepository_ShouldReturnPersonRepository()
        {

            var result = _sut.InstructorRepository;

            Assert.IsType<PersonRepository<Instructor>>(result);
        }

        [Fact]
        public void StudentRepository_ShouldReturnPersonRepository()
        {

            var result = _sut.StudentRepository;

            Assert.IsType<PersonRepository<Student>>(result);
        }

        [Fact]
        public void CourseRepository_ShouldReturnRepository()
        {

            var result = _sut.CourseRepository;

            Assert.IsType<Repository<Course>>(result);
        }

        [Fact]
        public void CourseAssignmentRepository_ShouldReturnRepository()
        {

            var result = _sut.CourseAssignmentRepository;

            Assert.IsType<Repository<CourseAssignment>>(result);
        }

        [Fact]
        public void OfficeAssignmentRepository_ShouldReturnRepository()
        {

            var result = _sut.OfficeAssignmentRepository;

            Assert.IsType<Repository<OfficeAssignment>>(result);
        }

        [Fact]
        public void EnrollmentRepository_ShouldReturnRepository()
        {

            var result = _sut.EnrollmentRepository;

            Assert.IsType<Repository<Enrollment>>(result);
        }

        [Fact]
        public async Task Commit_ShouldSaveChanges()
        {
            var instructor = new Instructor { FirstMidName = "Kim", LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11") };
            _sut.InstructorRepository.Add(instructor);

            _sut.Commit();

            var result = await _sut.InstructorRepository.GetByLastName("Abercrombie").ToListAsync();
            Assert.Equal(instructor.LastName, result[0].LastName);
        }
    }
}
