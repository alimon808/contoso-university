using ContosoUniversity.Tests;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.Api.Tests
{
    public class ApiIntegrationTests : BaseIntegrationTest<Startup>
    {
        [Fact(Skip = "Move to separate project")]
        public async Task DepartmentApi_ReturnsArrayOfDepartmentObjects()
        {
            var url = "/Departments";
            using (var th = InitTestServer())
            {
                var client = th.CreateClient();
                var response = await client.GetAsync(url);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                Assert.True(content.Contains("English"));
            }
        }
    }
}
