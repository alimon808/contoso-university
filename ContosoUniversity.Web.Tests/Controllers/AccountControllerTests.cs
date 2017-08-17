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
        private readonly FakeSignInManager _fakeSignInManager;

        public AccountControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _fakeUserManager = new FakeUserManager();
            _fakeSignInManager = new FakeSignInManager();

            _sut = new AccountController(_fakeUserManager, _fakeSignInManager);
        }

        [Fact]
        public void Login_ReturnsAViewResult_WithReturnUrlInViewData()
        {
            var returnUrl = "/Home/Index";
            var result =  _sut.Login(returnUrl);

            Assert.IsType(typeof(ViewResult), result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ContainsKey("ReturnUrl"));
        }

        [Fact]
        public async Task LoginPost_ReturnsARedirectResult()
        {
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl(It.IsAny<string>())).Returns(true);
            _sut.Url = mockUrl.Object;
            var model = new LoginViewModel { Email = "abc@example.com", Password = "abc", RememberMe = false };
            var returnUrl = "/Home/Index";
            

            var result = await _sut.LoginAsync(model, returnUrl);

            Assert.IsType(typeof(RedirectResult), result);
            Assert.Equal(returnUrl, ((RedirectResult)result).Url);
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
        public async Task RegisterPost_ReturnsARedirectResult()
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


        [Fact]
        public async Task RegisterPost_ReturnsAViewResult_WithInvalidModel()
        {
            
            var returnUrl = "/Departments/Index";
            var model = new RegisterViewModel { Email = "abc@example.com", Password = "P@ssw0rd!" };
            _sut.ModelState.AddModelError("myModelError", "my model error message");

            var result = await _sut.Register(model, returnUrl);

            Assert.IsType(typeof(ViewResult), result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("myModelError"));
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

        public class FakeSignInManager : SignInManager<ApplicationUser>
        {
            public FakeSignInManager()
                : base(new FakeUserManager(),
                      new HttpContextAccessor { HttpContext = new Mock<HttpContext>().Object },
                      new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                      null,
                      null)
            { }

            // solution from https://github.com/aspnet/Identity/issues/640
            public override Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
            {
                return Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success);
            }

            public override Task<Microsoft.AspNetCore.Identity.SignInResult> PasswordSignInAsync(string user, string password, bool isPersistent, bool lockoutOnFailure)
            {
                return Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success);
            }
        }
    }
}
