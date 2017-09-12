using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ContosoUniversity.Api.Controllers
{

    // api is modeled after example at https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?tabs=aspnet1x
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]Department newDepartment)
        {
            if (newDepartment == null || id != newDepartment.ID)
            {
                return BadRequest();
            }

            var oldDepartment = _departmentRepo.Get(newDepartment.ID).FirstOrDefault();
            if (oldDepartment == null)
            {
                return NotFound();
            }

            try
            {
                oldDepartment.Name = newDepartment.Name;
                oldDepartment.Budget = newDepartment.Budget;
                oldDepartment.InstructorID = newDepartment.InstructorID;

                _departmentRepo.Update(oldDepartment, oldDepartment.RowVersion);
                await _departmentRepo.SaveChangesAsync();
            }
            catch (Exception)
            {
                //todo: log excecption
                return BadRequest();
            }

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = _departmentRepo.Get(id).FirstOrDefault();
            if (department == null)
            {
                return NotFound();
            }

            _departmentRepo.Delete(department);
            await _departmentRepo.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
