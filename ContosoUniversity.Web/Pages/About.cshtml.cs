using ContosoUniversity.Common;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ContosoUniversity.Web.Pages
{
    public class AboutModel : PageModel
    {
        private readonly IRepository<Student> _studentRepo;
        public List<EnrollmentDateGroup> StudentEnrollments;

        public AboutModel(UnitOfWork<ApplicationContext> unitOfWork)
        {
            _studentRepo = unitOfWork.StudentRepository;
        }

        public void OnGet()
        {
            StudentEnrollments = Stats().Result;
        }

        private async Task<List<EnrollmentDateGroup>> Stats()
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

            return groups;
        }
    }
}