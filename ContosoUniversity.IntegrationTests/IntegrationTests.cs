using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests
{
    public class IntegrationTest : BaseIntegrationTest<Startup>
    {
        [Fact]
        public async Task Home_Index_ReturnsAViewResult()
        {
            var url = "/Home/Index";
            using (var th = InitTestServer())
            {
                var client = th.CreateClient();
                var response = await client.GetAsync(url);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
