using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.Data.Tests
{
    public class RepositoryTests
    {
        private Repository<Department, ApplicationContext> _sut;

        public RepositoryTests()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseInMemoryDatabase("TestDb");

            var context = new ApplicationContext(builder.Options);
            _sut = new Repository<Department, ApplicationContext>(context);
        }

        [Fact]
        public void Get_ReturnsEntityQueryable()
        {
            var result =_sut.Get(0);

            Assert.IsType(typeof(EntityQueryable<Department>), result);
        }

        [Fact]
        public void GetAll_ReturnsInternalDbSet()
        {
            var result = _sut.GetAll();

            Assert.IsType(typeof(InternalDbSet<Department>), result);
        }

        [Fact]
        public async Task Add_ShouldAddDepartment()
        {
            var department = new Department { InstructorID = 1, Name = "Physics", Budget = 0 };

            _sut.Add(department);
            await _sut.SaveChangesAsync();

            var departmentAdded = _sut.GetAll().Last();
            Assert.Equal(department.Name, departmentAdded.Name);
        }
        
        [Fact]
        public async Task AddAsync_ShouldAddDepartment()
        {
            var department = new Department { InstructorID = 1, Name = "Physics", Budget = 0 };

            await _sut.AddAsync(department);
            await _sut.SaveChangesAsync();

            var departmentAdded = _sut.GetAll().Last();
            Assert.Equal(department.Name, departmentAdded.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveDepartment()
        {
            await ResetDb();
            var department = new Department { InstructorID = 1, Name = "Physics", Budget = 0 };
            var count = _sut.GetAll().Count();
            Assert.Equal(0, count);

            await _sut.AddAsync(department);
            await _sut.SaveChangesAsync();

            var departmentAdded = _sut.GetAll().Last();
            count = _sut.GetAll().Count();
            Assert.Equal(department.Name, departmentAdded.Name);
            Assert.Equal(1, count);

            _sut.Delete(departmentAdded);
            await _sut.SaveChangesAsync();

            count = _sut.GetAll().Count();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task Update_ShouldModifyDepartment()
        {
            await ResetDb();
            var department = new Department { InstructorID = 1, Name = "Physics", Budget = 0 };
            var count = _sut.GetAll().Count();
            Assert.Equal(0, count);

            await _sut.AddAsync(department);
            await _sut.SaveChangesAsync();

            var departmentAdded = _sut.GetAll().Last();
            count = _sut.GetAll().Count();
            Assert.Equal(department.Name, departmentAdded.Name);
            Assert.Equal(1, count);

            var departmentToUpdate = departmentAdded;
            departmentToUpdate.Name = "Physics II";

            _sut.Update(departmentToUpdate, departmentAdded.RowVersion);
            await _sut.SaveChangesAsync();

            Assert.Equal("Physics II", departmentAdded.Name);
        }

        private async Task ResetDb()
        {
            var departments = _sut.GetAll().ToArray();
            foreach (var item in departments)
            {
                _sut.Delete(item);
            }
            await _sut.SaveChangesAsync();
        }
    }
}
