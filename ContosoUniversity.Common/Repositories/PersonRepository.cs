using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using System;
using System.Linq;
using ContosoUniversity.Data;

namespace ContosoUniversity.Common.Repositories
{
    public class PersonRepository<T> : Repository<T>, IPersonRepository<T> where T : Person
    {
        public PersonRepository(ApplicationContext context) : base(context)
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
