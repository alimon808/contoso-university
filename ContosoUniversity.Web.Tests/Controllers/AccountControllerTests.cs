using ContosoUniversity.Identity;
using ContosoUniversity.Services;
using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Web.Helpers;
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
using System.IO;
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
        private readonly Mock<IUrlHelperAdaptor> _mockUrlHelperAdaptor;
        private readonly Mock<IEmailSender> _mockEmailSender;

        public AccountControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _fakeUserManager = new FakeUserManager();
            _fakeSignInManager = new FakeSignInManager();
            _mockUrlHelperAdaptor = new Mock<IUrlHelperAdaptor>();
            _mockEmailSender = new Mock<IEmailSender>();

            _sut = new AccountController(_fakeUserManager, _fakeSignInManager, _mockEmailSender.Object, _mockUrlHelperAdaptor.Object);
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
            

            var result = await _sut.Login(model, returnUrl);

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

            // moq HttpContext.Request.Scheme
            // https://stackoverflow.com/questions/41400030/mock-httpcontext-for-unit-testing-a-net-core-mvc-controller
            var context = new Mock<HttpContext>();
            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext();

            _mockUrlHelperAdaptor.Setup(m => m.Action(It.IsAny<IUrlHelper>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).Returns("confirmemialurl");
            _mockEmailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));
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

        [Fact]
        public async Task LogoutPost_ReturnsARedirectToActionResult()
        {
            var result = await _sut.Logout();

            Assert.IsType(typeof(RedirectToActionResult), result);
            Assert.Equal("Home", ((RedirectToActionResult)result).ControllerName);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public void ResetPassword_ReturnsAViewResult()
        {
            var result = _sut.ResetPassword();

            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public async Task ResetPasswordPost_ReturnsARedirectToActionResult_ResetPasswordConfirmation()
        {
            var model = new ResetPasswordViewModel();

            var result = await _sut.ResetPassword(model);

            Assert.IsType(typeof(RedirectToActionResult), result);
            Assert.Equal("ResetPasswordConfirmation", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task ResetPasswordPost_ReturnsAViewResult_WithInvalidModel()
        {
            var model = new ResetPasswordViewModel();
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");

            var result = await _sut.ResetPassword(model);

            Assert.IsType(typeof(ViewResult), result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("mymodelerror"));
        }

        [Fact]
        public void ForgotPassword_ReturnsAViewResult()
        {
            var result = _sut.ForgotPassword();

            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public async Task ForgotPasswordPost_ReturnsAViewResult_WithInvalidModel()
        {
            var vm = new ForgotPasswordViewModel { };
            _sut.ModelState.AddModelError("myerror", "my error message");

            var result = await _sut.ForgotPassword(vm);

            Assert.IsType(typeof(ViewResult), result);

            var viewData = ((ViewResult)result).ViewData;
            Assert.True(viewData.ModelState.ContainsKey("myerror"));
        }

        [Fact]
        public async Task ForgotPasswordPost_ReturnsAViewResult_WithForgotPasswordConfirmation()
        {
            var vm = new ForgotPasswordViewModel { Email = "a@example.com" };
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl(It.IsAny<string>())).Returns(true);
            var context = new Mock<HttpContext>();
            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sut.Url = mockUrl.Object;
            _mockUrlHelperAdaptor.Setup(m => m.Action(It.IsAny<IUrlHelper>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).Returns("confirmemialurl");
            _mockEmailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));

            var result = await _sut.ForgotPassword(vm);

            Assert.IsType(typeof(ViewResult), result);

            Assert.Equal("ForgotPasswordConfirmation", ((ViewResult)result).ViewName);
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
            public override Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
            {
                return Task.FromResult("token string");
            }
            public override Task<ApplicationUser> FindByEmailAsync(string email)
            {
                return Task.FromResult(new ApplicationUser());
            }
            public override Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
            {
                return Task.FromResult("reset-password-token");
            }
            public override Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
            {
                return Task.FromResult(true);
            }
            public override Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
            {
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

            public override Task SignOutAsync()
            {
                return Task.FromResult(0);
            }
        }
    }
}
