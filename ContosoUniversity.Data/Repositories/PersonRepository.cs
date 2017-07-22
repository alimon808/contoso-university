using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;
using System;
using System.Linq;

namespace ContosoUniversity.Data
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
