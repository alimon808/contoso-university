using ContosoUniversity.Tests;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContosoUniversity.Web.Tests
{
    public class IntegrationTest : BaseIntegrationTest<Startup>
    {
        [Fact(Skip = "Local Test Only")]
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

        [Theory(Skip = "Local Test Only")]
        [InlineData("Departments")]
        //todo: integration testing on Courses, Instructors, and Students
        //[InlineData("Courses")]
        //[InlineData("Instructors")]
        //[InlineData("Students")]
        public async Task CRUD(string controller)
        {

            var url = $"/{controller}/Create";
            using (var th = InitTestServer())
            {
                // get create form to retrieve antiforgery token
                var client = th.CreateClient();
                var response = await client.GetAsync(url);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // create entity 
                var cookieVal = response.GetAntiForgeryCookie(AntiForgeryCookieName).ToString();
                var formTokenVal = await response.GetAntiForgeryFormToken(AntiForgeryFormTokenName);
                client.DefaultRequestHeaders.Add("Cookie", cookieVal);

                var departmentToAdd = new DepartmentCreateViewModel { Name = "Economics", Budget = 100000, StartDate = DateTime.Parse("2007-09-01"), InstructorID = 4 };
                response = await client.PostFormDataAsync<DepartmentCreateViewModel>(url, departmentToAdd, formTokenVal);

                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.Contains("newid", response.Headers.Location.ToString());

                // verify entity was added
                var uri = new Uri(client.BaseAddress, response.Headers.Location.ToString());
                var queryDictionary = QueryHelpers.ParseQuery(uri.Query);
                var newid = queryDictionary["newid"];

                response = await client.GetAsync($"/{controller}/Details/{newid}");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                //edit new entity
                url = $"/{controller}/Edit/{newid}";
                response = await client.GetAsync(url);
                formTokenVal = await response.GetAntiForgeryFormToken(AntiForgeryFormTokenName);
                var rowVersion = await response.GetRowVersion();
                var departmentToEdit = new DepartmentEditViewModel { ID = int.Parse(newid), Name = "Economics 2", Budget = 999, StartDate = DateTime.UtcNow, RowVersion = rowVersion };
                response = await client.PostFormDataAsync<DepartmentEditViewModel>(url, departmentToEdit, formTokenVal);

                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

                // delete new entity
                url = $"/{controller}/delete/{newid}";
                response = await client.GetAsync(url);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                formTokenVal = await response.GetAntiForgeryFormToken(AntiForgeryFormTokenName);
                var departmentToDelete = new DepartmentDetailsViewModel { ID = int.Parse(newid) };
                response = await client.PostFormDataAsync<DepartmentDetailsViewModel>(url, departmentToDelete, formTokenVal);
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            }
        }



        //todo: concurrency tests
    }
}
