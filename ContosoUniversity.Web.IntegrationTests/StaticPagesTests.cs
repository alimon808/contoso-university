using Xunit;
using System.Threading.Tasks;

namespace ContosoUniversity.Web.IntegrationTests
{
    public class StaticPagesTests : IClassFixture<CustomWebApplicationFactory<ContosoUniversity.Startup>>
    {
        private readonly CustomWebApplicationFactory<ContosoUniversity.Startup> _factory;
        public StaticPagesTests(CustomWebApplicationFactory<ContosoUniversity.Startup> factory) => _factory = factory;

        [Theory]
        [InlineData("/")]
        [InlineData("/contact")]
        public async Task GetPages_ReturnSuccess(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
