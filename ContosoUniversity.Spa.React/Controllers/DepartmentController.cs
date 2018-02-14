using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ContosoUniversity_Spa_React.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        private static string[] Departments = new[]
        {
            "Engineering", "English", "Physics"
        };

        public IEnumerable<string> Get()
        {
            return Departments;
        }
    }
}