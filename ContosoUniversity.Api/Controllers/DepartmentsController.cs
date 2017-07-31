using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;

namespace ContosoUniversity.Api.Controllers
{
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
    }
}
