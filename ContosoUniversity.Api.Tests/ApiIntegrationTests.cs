using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.Api.Tests
{
    public class ApiIntegrationTests : BaseIntegrationTest<Startup>
    {
        [Fact]
        public async Task DepartmentApi_ReturnsArrayOfDepartmentObjects()
        {
            var url = "/Departments";
            using (var th = InitTestServer())
            {
                var client = th.CreateClient();
                var response = await client.GetAsync(url);

                var content = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
