using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Api.Controllers
{

    // api is modeled after https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?tabs=aspnet1x
    [Route("[controller]")]
    public class DepartmentsController : Controller
    {
        private IRepository<Department> _departmentRepo;

        public DepartmentsController(IRepository<Department> departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        [HttpGet]
        public IEnumerable<Department> GetAll()
        {
            return _departmentRepo.GetAll().ToList();
        }

        [HttpGet("{id}", Name ="GetDepartment")]
        public IActionResult GetById(int id)
        {
            var department = _departmentRepo.Get(id).FirstOrDefault();
            if (department == null)
            {
                return NotFound();
            }

            return new ObjectResult(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department department)
        {
            if (department == null)
            {
                return BadRequest();
            }

            await _departmentRepo.AddAsync(department);
            await _departmentRepo.SaveChangesAsync();

            return CreatedAtRoute("GetDepartment", new { id = department.ID }, department);
        }
    }
}
