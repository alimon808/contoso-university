using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests
{
    public class ApiIntegrationTests : BaseIntegrationTest<ContosoUniversity.Api.Startup>
    {
        [Fact]
        public async Task DepartmentApi_ReturnsListOfDepartments()
        {
            //var url = "/api/Departments";
            var url = "http://www.google.com";
            using (var th = InitTestServer())
            {
                var client = th.CreateClient();
                var response = await client.GetAsync(url);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
