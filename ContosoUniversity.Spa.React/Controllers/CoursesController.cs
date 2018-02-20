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
    public class CoursesController : Controller
    {
        private readonly IRepository<Course> _coursesRepo;
        private readonly IMapper _mapper;

        public CoursesController(UnitOfWork<ApiContext> unitOfWork, IMapper mapper)
        {
            _coursesRepo = unitOfWork.CourseRepository;
            _mapper = mapper;
        }

        public IEnumerable<Course> Get()
        {
            return _coursesRepo.GetAll().ToArray();
        }
    }
}