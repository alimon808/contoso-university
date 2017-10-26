using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;
using AutoMapper;
using ContosoUniversity.Common.DTO;

namespace ContosoUniversity.Api.Controllers
{

    // api is modeled after example at https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?tabs=aspnet1x
    [Route("[controller]")]
    public class DepartmentsController : Controller
    {
        private IRepository<Department> _departmentRepo;
        private readonly IMapper _mapper;

        public DepartmentsController(IRepository<Department> departmentRepo, IMapper mapper)
        {
            _departmentRepo = departmentRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<DepartmentDTO> GetAll()
        {
            var query = _departmentRepo.GetAll()
                .Select(d => _mapper.Map<DepartmentDTO>(d));

            return query.ToList();
        }

        [HttpGet("{id}", Name = "GetDepartment")]
        public IActionResult GetById(int id)
        {
            var dto = _departmentRepo.Get(id)
                .Select(d => _mapper.Map<DepartmentDTO>(d))
                .FirstOrDefault();
            if (dto == null)
            {
                return NotFound();
            }

            return new ObjectResult(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var department = _mapper.Map<Department>(dto);

            await _departmentRepo.AddAsync(department);
            await _departmentRepo.SaveChangesAsync();

            return CreatedAtRoute("GetDepartment", new { id = department.ID }, _mapper.Map<DepartmentDTO>(department));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]DepartmentDTO dto)
        {
            if (dto == null || id != dto.ID)
            {
                return BadRequest();
            }

            var newDepartment = _mapper.Map<Department>(dto);

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
