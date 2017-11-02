using ContosoUniversity.Common;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Tests
{
    public static class MockingHelper
    {
        public static Mock<DbSet<TEntity>> AsMockDbSet<TEntity>(this IQueryable<TEntity> data)
            where TEntity : class
        {
            var mockSet = new Mock<DbSet<TEntity>>();
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TEntity>(data.Provider));
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));
            return mockSet;
        }

        public static Mock<DbSet<TEntity>> AsMockDbSet<TEntity>(this List<TEntity> data)
            where TEntity : class
        {
            Mock<DbSet<TEntity>> mockDbSet = data.AsQueryable().AsMockDbSet();

            return mockDbSet;
        }

        public static Mock<IRepository<TEntity>> AsMockRepository<TEntity>(this List<TEntity> data) where TEntity : BaseEntity
        {
            var mockRepository = new Mock<IRepository<TEntity>>();
            mockRepository.Setup(c => c.AddAsync(It.IsAny<TEntity>()))
                .Callback<TEntity>(d => 
                {
                    d.ID = data.Count() + 1;
                    data.Add(d);
                })
                .Returns(Task.FromResult(0));
            mockRepository.Setup(c => c.Delete(It.IsAny<TEntity>())).Callback<TEntity>(d => data.Remove(d));
            mockRepository.Setup(c => c.GetAll()).Returns(data.AsMockDbSet<TEntity>().Object.AsQueryable<TEntity>());
            mockRepository.Setup(c => c.Get(It.IsAny<int>())).Returns<int>(id => data.AsMockDbSet<TEntity>().Object.Where(s => s.ID == id).AsQueryable<TEntity>());
            return mockRepository;
        }

        public static Mock<IPersonRepository<TEntity>> AsMockPersonRepository<TEntity>(this List<TEntity> data) where TEntity : Person
        {
            var mockRepository = new Mock<IPersonRepository<TEntity>>();
            mockRepository.Setup(c => c.AddAsync(It.IsAny<TEntity>())).Callback<TEntity>(d => data.Add(d)).Returns(Task.FromResult(0));
            mockRepository.Setup(c => c.Delete(It.IsAny<TEntity>())).Callback<TEntity>(d => data.Remove(d));
            mockRepository.Setup(c => c.GetAll()).Returns(data.AsMockDbSet<TEntity>().Object.AsQueryable<TEntity>());
            mockRepository.Setup(c => c.Get(It.IsAny<int>())).Returns<int>(id => data.AsMockDbSet<TEntity>().Object.Where(s => s.ID == id).AsQueryable<TEntity>());

            return mockRepository;
        }
    }
}
