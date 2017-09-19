using ContosoUniversity.Data.Entities;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Data.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> Get(int id);
        void Add(T entity);
        Task AddAsync(T entity);
        void Update(T entity, byte[] rowVersion);
        void Delete(T entity);
        Task<int> ExecuteSqlCommandAsync(string queryString);
        Task SaveChangesAsync();
        DbConnection GetDbConnection();
    }
}
