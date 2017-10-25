using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Models.SchoolViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Common;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;

namespace ContosoUniversity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Student> _studentRepo;

        public HomeController(IRepository<Student> studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            List<EnrollmentDateGroup> groups = new List<EnrollmentDateGroup>();
            var conn = _studentRepo.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    // todo: read from configuration
                    var dbSchema = "Contoso.";
                    if (ContosoUniversity.Common.OperatingSystem.IsMacOs())
                    {
                        dbSchema = string.Empty;
                    }
                    string query = $"SELECT EnrollmentDate, COUNT(*) AS StudentCount FROM {dbSchema}Person WHERE Discriminator = 'Student' GROUP BY EnrollmentDate";
                    command.CommandText = query;
                    DbDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new EnrollmentDateGroup
                            {
                                EnrollmentDate = reader.GetDateTime(0),
                                StudentCount = reader.GetInt32(1)
                            };
                            groups.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }

            return View(groups);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
