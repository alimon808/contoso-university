using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using System;
using System.Linq;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Common.Repositories
{
    public class PersonRepository<T, TContext> : Repository<T, TContext>, IPersonRepository<T> where T : Person where TContext : DbContext
    {
        public PersonRepository(TContext context) : base(context)
        {
        }

        public IQueryable<T> GetByLastName(string lastName)
        {
            return base.GetAll().Where(p => p.LastName == lastName);
        }

        private void IsEntityNull(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
        }
    }
}
