using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.IntegrationTests
{
    public class IntegrationTests : BaseIntegrationTest<Startup>
    {
        [Fact]
        public async Task HomeControllerIndex_ReturnsAViewResult()
        {
            //var url = "http://www.google.com";
            var url = "/Home/Index";
            using (var th = InitTestServer())
            {
                var client = th.CreateClient();
                var response = await client.GetAsync(url);

                var responseString = await response.Content.ReadAsStringAsync();
                Assert.True(responseString.Contains("English"));
            }
        }
    }
}
