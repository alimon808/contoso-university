using ContosoUniversity.Identity;
using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Web.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly ITestOutputHelper _output;
        AccountController _sut;
        private readonly FakeUserManager _fakeUserManager;
        public AccountControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _fakeUserManager = new FakeUserManager();
            _sut = new AccountController(_fakeUserManager);
        }
        
        [Fact]
        public void Register_ReturnsAViewResult_WithReturnUrlInViewData()
        {
            var returnUrl = "/Home/Index";
            var result = _sut.Register(returnUrl);

            Assert.IsType(typeof(ViewResult), result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("ReturnUrl"));
        }

        [Fact]
        public async Task Register_ReturnsARedirectResult()
        {
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl(It.IsAny<string>())).Returns(true);
            _sut.Url = mockUrl.Object;
            var returnUrl = "/Departments/Index";
            var model = new RegisterViewModel { Email = "abc@example.com", Password = "P@ssw0rd!" };

            var result = await _sut.Register(model, returnUrl);

            Assert.IsType(typeof(RedirectResult), result);
            Assert.Equal(returnUrl, ((RedirectResult)result).Url);
        }

        // original code from https://github.com/aspnet/Identity/issues/344
        public class FakeUserManager : UserManager<ApplicationUser>
        {
            public FakeUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                      new Mock<IOptions<IdentityOptions>>().Object,
                      new Mock<IPasswordHasher<ApplicationUser>>().Object,
                      new IUserValidator<ApplicationUser>[0],
                      new IPasswordValidator<ApplicationUser>[0],
                      new Mock<ILookupNormalizer>().Object,
                      new Mock<IdentityErrorDescriber>().Object,
                      new Mock<IServiceProvider>().Object,
                      new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
            { }

            public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
            {
                // solution from https://stackoverflow.com/questions/26269104/how-to-construct-identityresult-with-success-true
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}
