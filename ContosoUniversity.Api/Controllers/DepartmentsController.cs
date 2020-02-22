using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;
using AutoMapper;
using ContosoUniversity.Common.DTO;
using ContosoUniversity.Api.DTO;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace ContosoUniversity.Api.Controllers
{
    // api modeled after example at https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?tabs=aspnet1x
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DepartmentsController : Controller
    {
        private IRepository<Department> _departmentRepo;
        private IRepository<Instructor> _instructorRepo;
        private readonly IMapper _mapper;

        public DepartmentsController(UnitOfWork<ApiContext> unitOfWork, IMapper mapper)
        {
            _departmentRepo = unitOfWork.DepartmentRepository;
            _instructorRepo = unitOfWork.InstructorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
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

        /// <summary>
        /// Creates a Department.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Departments
        ///     {
        ///        "instructorID": 1,
        ///        "name": "Physics",
        ///        "budget": 100,
        ///        "startDate": "2017-10-29T15:24:14.300Z"
        ///     }
        ///
        /// </remarks>
        /// <param name="dto"></param>
        /// <returns>A newly-created TodoItem</returns>
        /// <response code="201">Returns the newly-created item</response>
        /// <response code="400">If the item is null or model state is invalid</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDTO dto)
        {
            // check if instructor exists
            if (!_instructorRepo.Get(dto.InstructorID).Any())
            {
                ModelState.AddModelError("InstructorID", "InstructorID does not exist.");
                return BadRequest(ModelState);
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
