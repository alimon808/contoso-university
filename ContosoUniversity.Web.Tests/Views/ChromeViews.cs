using ContosoUniversity.Tests;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace ContosoUniversity.Web.Tests.Views
{
    public class ChromeViews : BaseIntegrationTest<Startup>
    {
        private readonly Uri _baseUrl;

        public ChromeViews()
        {
            _baseUrl = new Uri("https://localhost:44368/");
        }

        [Fact(Skip = "Chrome UI")]
        public void HomeIndex_ShouldHaveAJumbotron_WithH1()
        {
            var home = _baseUrl;
            using (var driver = new ChromeDriver
                  (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
            {
                driver.Navigate().GoToUrl(home);
                var jumbotron = driver.FindElementByClassName("jumbotron");
                var h1 = jumbotron.FindElement(By.TagName("h1"));
                Assert.Equal("Contoso University", h1.Text);
            }
        }
    }
}
