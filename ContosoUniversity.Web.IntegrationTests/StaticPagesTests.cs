using Xunit;
using System.Threading.Tasks;

namespace ContosoUniversity.Web.IntegrationTests
{
    public class StaticPagesTests : IClassFixture<CustomWebApplicationFactory<ContosoUniversity.Startup>>
    {
        private readonly CustomWebApplicationFactory<ContosoUniversity.Startup> _factory;
        public StaticPagesTests(CustomWebApplicationFactory<ContosoUniversity.Startup> factory) => _factory = factory;

        #region Razor Pages
        [Theory]
        [InlineData("/")]
        [InlineData("/contact")]
        public async Task GetPages_ReturnSuccess(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Mvc Views
        [Theory]
        [InlineData("/students")]
        [InlineData("/students/details/1")]
        [InlineData("/courses")]
        [InlineData("/courses/details/1")]
        [InlineData("/instructors")]
        [InlineData("/instructors/details/2")]
        [InlineData("/departments")]
        [InlineData("/departments/details/1")]
        public async Task GetControllersIndexAndDetails_ReturnSuccess(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
