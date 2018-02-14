using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity_Spa_React.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        private IRepository<Department> _departmentRepo;

        public DepartmentController(UnitOfWork<ApiContext> unitOfWork)
        {
            _departmentRepo = unitOfWork.DepartmentRepository;
        }

        public IEnumerable<string> Get()
        {
            return _departmentRepo.GetAll().Select(d => d.Name).ToArray();
        }
    }
}