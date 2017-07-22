using ContosoUniversity.Data.Entities;
using System.Linq;

namespace ContosoUniversity.Data.Interfaces
{
    public interface IPersonRepository<T> : IRepository<T> where T : Person
    {
        IQueryable<T> GetByLastName(string lastName);
    }
}
