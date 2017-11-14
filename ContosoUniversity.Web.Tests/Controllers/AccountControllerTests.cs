using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using ContosoUniversity.Web.Controllers;
using ContosoUniversity.Web.Helpers;
using ContosoUniversity.Web.ViewModels;

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
        private readonly Mock<ISmsSender> _mockSmsSender;

        public AccountControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _fakeUserManager = new FakeUserManager();
            _fakeSignInManager = new FakeSignInManager();
            _mockUrlHelperAdaptor = new Mock<IUrlHelperAdaptor>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockSmsSender = new Mock<ISmsSender>();

            _sut = new AccountController(_fakeUserManager, _fakeSignInManager, _mockEmailSender.Object, _mockSmsSender.Object, _mockUrlHelperAdaptor.Object);
        }

        [Fact]
        public void Login_ReturnsAViewResult_WithReturnUrlInViewData()
        {
            var returnUrl = "/Home/Index";
            var result = _sut.Login(returnUrl);

            Assert.IsType<ViewResult>(result);

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

            Assert.IsType<RedirectResult>(result);
            Assert.Equal(returnUrl, ((RedirectResult)result).Url);
        }

        [Fact]
        public async Task LoginPost_ReturnsARedirectToActionResult_RequiresTwoFactorAuthentication()
        {
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl(It.IsAny<string>())).Returns(true);
            _sut.Url = mockUrl.Object;
            _fakeSignInManager.SignInResult = Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired;
            var model = new LoginViewModel { Email = "abc@example.com", Password = "abc", RememberMe = false };
            var returnUrl = "/Home/Index";

            var result = await _sut.Login(model, returnUrl);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("SendCode", actionName);

            var routeValues = ((RedirectToActionResult)result).RouteValues;
            Assert.Equal(returnUrl, routeValues["ReturnUrl"]);
            Assert.Equal(model.RememberMe, routeValues["RememberMe"]);
        }

        [Fact]
        public async Task LoginPost_ReturnsALockoutViewResult()
        {
            _fakeSignInManager.SignInResult = Microsoft.AspNetCore.Identity.SignInResult.LockedOut;
            var model = new LoginViewModel { Email = "abc@example.com", Password = "abc", RememberMe = false };
            var returnUrl = "/Home/Index";

            var result = await _sut.Login(model, returnUrl);

            Assert.IsType<ViewResult>(result);

            var viewName = ((ViewResult)result).ViewName;
            Assert.Equal("Lockout", viewName);
        }

        [Fact]
        public void Register_ReturnsAViewResult_WithReturnUrlInViewData()
        {
            var returnUrl = "/Home/Index";
            var result = _sut.Register(returnUrl);

            Assert.IsType<ViewResult>(result);

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
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _mockUrlHelperAdaptor.Setup(m => m.Action(It.IsAny<IUrlHelper>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).Returns("confirmemialurl");
            _mockEmailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));
            var returnUrl = "/Departments/Index";
            var model = new RegisterViewModel { Email = "abc@example.com", Password = "P@ssw0rd!" };

            var result = await _sut.Register(model, returnUrl);

            Assert.IsType<RedirectResult>(result);
            Assert.Equal(returnUrl, ((RedirectResult)result).Url);
        }

        [Fact]
        public async Task RegisterPost_ReturnsAViewResult_WithInvalidModel()
        {

            var returnUrl = "/Departments/Index";
            var model = new RegisterViewModel { Email = "abc@example.com", Password = "P@ssw0rd!" };
            _sut.ModelState.AddModelError("myModelError", "my model error message");

            var result = await _sut.Register(model, returnUrl);

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("myModelError"));
        }

        [Fact]
        public async Task LogoutPost_ReturnsARedirectToActionResult()
        {
            var result = await _sut.Logout();

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", ((RedirectToActionResult)result).ControllerName);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public void ResetPassword_ReturnsAViewResult()
        {
            var result = _sut.ResetPassword();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task SendCode_ReturnsAViewResult()
        {
            _fakeUserManager.TwoFactorProviders = new List<string> { "Email" };

            var result = await _sut.SendCode();

            Assert.IsType<ViewResult>(result);

            var model = (SendCodeViewModel)((ViewResult)result).Model;
            Assert.False(model.RememberMe);
            Assert.Null(model.ReturnUrl);
            Assert.Equal(1, model.Providers.Count);
        }

        [Fact]
        public async Task SendCodePost_ReturnsAViewResult_WithInvalidModel()
        {
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");
            var model = new SendCodeViewModel { SelectedProvider = "Email", RememberMe = false, ReturnUrl = null };

            var result = await _sut.SendCode(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task SendCodePost_ReturnsAErrorViewResult()
        {
            _fakeSignInManager.ApplicationUser = null;
            var model = new SendCodeViewModel { SelectedProvider = "Email", RememberMe = false, ReturnUrl = null };

            var result = await _sut.SendCode(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", ((ViewResult)result).ViewName);
        }

        [Fact]
        public async Task SendCodePost_ReturnsARedirectToActionResult_ToVerifyCode_UsingEmailProvider()
        {
            _mockEmailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));
            var model = new SendCodeViewModel { SelectedProvider = "Email", RememberMe = false, ReturnUrl = null };

            var result = await _sut.SendCode(model);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("VerifyCode", actionName);

            var routeValues = ((RedirectToActionResult)result).RouteValues;
            Assert.Equal(model.SelectedProvider, routeValues["Provider"]);
            Assert.Equal(model.ReturnUrl, routeValues["ReturnUrl"]);
            Assert.Equal(model.RememberMe, routeValues["RememberMe"]);
        }

        [Fact]
        public async Task SendCodePost_ReturnsARedirectToActionResult_ToVerifyCode_UsingPhoneProvider()
        {
            _mockSmsSender.Setup(m => m.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));
            var model = new SendCodeViewModel { SelectedProvider = "Phone", RememberMe = false, ReturnUrl = null };

            var result = await _sut.SendCode(model);

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("VerifyCode", actionName);

            var routeValues = ((RedirectToActionResult)result).RouteValues;
            Assert.Equal(model.SelectedProvider, routeValues["Provider"]);
            Assert.Equal(model.ReturnUrl, routeValues["ReturnUrl"]);
            Assert.Equal(model.RememberMe, routeValues["RememberMe"]);
        }

        [Fact]
        public async Task VerfifyCode_ReturnsAViewResult()
        {
            var provider = "Email";
            var rememberMe = false;
            string returnUrl = null;

            var result = await _sut.VerifyCode(provider, rememberMe, returnUrl);

            Assert.IsType<ViewResult>(result);
            var model = (VerifyCodeViewModel)((ViewResult)result).Model;
            Assert.Equal(provider, model.Provider);
            Assert.Equal(rememberMe, model.RememberMe);
            Assert.Equal(returnUrl, model.ReturnUrl);
        }

        [Fact]
        public async Task VerfifyCode_ReturnsAErrorViewResult()
        {
            _fakeSignInManager.ApplicationUser = null;

            var result = await _sut.VerifyCode("", false, null);

            Assert.IsType<ViewResult>(result);

            var viewName = ((ViewResult)result).ViewName;
            Assert.Equal("Error", viewName);
        }

        [Fact]
        public async Task VerfifyCodePost_ReturnsARedirectToActionResult_ManageIndex()
        {
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl(It.IsAny<string>())).Returns(false);
            _sut.Url = mockUrl.Object;
            var model = new VerifyCodeViewModel { };

            var result = await _sut.VerifyCode(model);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
            Assert.Equal("Manage", ((RedirectToActionResult)result).ControllerName);
        }

        [Fact]
        public async Task VerfifyCodePost_ReturnsAViewResult_WithInvalidCode()
        {
            var model = new VerifyCodeViewModel { };
            _fakeSignInManager.SignInResult = Microsoft.AspNetCore.Identity.SignInResult.Failed;

            var result = await _sut.VerifyCode(model);

            Assert.IsType<ViewResult>(result);
            Assert.Single(((ViewResult)result).ViewData.ModelState);
        }

        [Fact]
        public async Task VerfifyCodePost_ReturnsAViewResult_WithInvalidModel()
        {
            var model = new VerifyCodeViewModel { };
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");

            var result = await _sut.VerifyCode(model);

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("mymodelerror"));
        }

        [Fact]
        public async Task VerfifyCodePost_ReturnsALockoutViewResult()
        {
            var model = new VerifyCodeViewModel { };
            _fakeSignInManager.SignInResult = Microsoft.AspNetCore.Identity.SignInResult.LockedOut;

            var result = await _sut.VerifyCode(model);

            Assert.IsType<ViewResult>(result);

            var viewName = ((ViewResult)result).ViewName;
            Assert.Equal("Lockout", viewName);
        }

        [Fact]
        public async Task ResetPasswordPost_ReturnsARedirectToActionResult_ResetPasswordConfirmation()
        {
            var model = new ResetPasswordViewModel();

            var result = await _sut.ResetPassword(model);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResetPasswordConfirmation", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task ResetPasswordPost_ReturnsAViewResult_WithInvalidModel()
        {
            var model = new ResetPasswordViewModel();
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");

            var result = await _sut.ResetPassword(model);

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("mymodelerror"));
        }

        [Fact]
        public void ForgotPassword_ReturnsAViewResult()
        {
            var result = _sut.ForgotPassword();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ForgotPasswordPost_ReturnsAViewResult_WithInvalidModel()
        {
            var vm = new ForgotPasswordViewModel { };
            _sut.ModelState.AddModelError("myerror", "my error message");

            var result = await _sut.ForgotPassword(vm);

            Assert.IsType<ViewResult>(result);

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
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _sut.Url = mockUrl.Object;
            _mockUrlHelperAdaptor.Setup(m => m.Action(It.IsAny<IUrlHelper>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).Returns("confirmemialurl");
            _mockEmailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));

            var result = await _sut.ForgotPassword(vm);

            Assert.IsType<ViewResult>(result);

            Assert.Equal("ForgotPasswordConfirmation", ((ViewResult)result).ViewName);
        }

        // original code from https://github.com/aspnet/Identity/issues/344
        // todo: move into separate file
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

            public IList<string> TwoFactorProviders { get; set; } = new List<string> { "Email", "Phone" };

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
            public override Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user)
            {
                return Task.FromResult(this.TwoFactorProviders);
            }
            public override Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
            {
                return Task.FromResult("two-factor-token");
            }
            public override Task<string> GetEmailAsync(ApplicationUser user)
            {
                return Task.FromResult("admin@contoso.com");
            }
            public override Task<string> GetPhoneNumberAsync(ApplicationUser user)
            {
                return Task.FromResult("224-555-0123");
            }
        }

        // todo: move into separate file
        public class FakeSignInManager : SignInManager<ApplicationUser>
        {
            public Microsoft.AspNetCore.Identity.SignInResult SignInResult { get; set; } = Microsoft.AspNetCore.Identity.SignInResult.Success;
            public ApplicationUser ApplicationUser { get; set; } = new ApplicationUser { Email = "admin@contoso.com" };

            public FakeSignInManager()
                : base(new FakeUserManager(),
                      new HttpContextAccessor { HttpContext = new Mock<HttpContext>().Object },
                      new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                      null,
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
                return Task.FromResult(this.SignInResult);
            }

            public override Task<ApplicationUser> GetTwoFactorAuthenticationUserAsync()
            {
                return Task.FromResult(this.ApplicationUser);
            }
            public override Task SignOutAsync()
            {
                return Task.FromResult(0);
            }
            public override Task<Microsoft.AspNetCore.Identity.SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
            {
                return Task.FromResult(this.SignInResult);
            }
        }
    }
}
