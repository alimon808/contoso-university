using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;

namespace ContosoUniversity.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;

        public Repository()
        {
        }

        public Repository(ApplicationContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public IQueryable<T> Get(int id)
        {
            return entities.Where(s => s.ID == id).AsQueryable<T>();
        }

        public IQueryable<T> GetAll()
        {
            return entities.AsQueryable<T>();
        }

        public void Add(T entity)
        {
            context.Add<T>(entity);
        }

        public async Task AddAsync(T entity)
        {
            await entities.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            entities.Remove(entity);
        }

        public void Update(T entity, byte[] rowVersion)
        {
            context.Entry(entity).Property("RowVersion").OriginalValue = rowVersion;
            context.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
        
        public async Task<int> ExecuteSqlCommandAsync(string queryString)
        {
            return await context.Database.ExecuteSqlCommandAsync(queryString);
        }

        public DbConnection GetDbConnection()
        {
            return context.Database.GetDbConnection();
        }

    }
}
