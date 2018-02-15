using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data.Entities;
using System.Linq;
using ContosoUniversity.Common;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Common.DTO;
using AutoMapper;

namespace ContosoUniversity_Spa_React.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IRepository<Department> _departmentRepo;
        private readonly IMapper _mapper;

        public DepartmentController(UnitOfWork<ApiContext> unitOfWork, IMapper mapper)
        {
            _departmentRepo = unitOfWork.DepartmentRepository;
            _mapper = mapper;
        }

        public IEnumerable<string> Get()
        {
            return _departmentRepo.GetAll().Select(d => d.Name).ToArray();
        }

        [HttpGet("[action]")]
        public IEnumerable<DepartmentDTO> Details()
        {
            return _departmentRepo.GetAll().Select(d => _mapper.Map<DepartmentDTO>(d)).ToArray();
        }
    }
}