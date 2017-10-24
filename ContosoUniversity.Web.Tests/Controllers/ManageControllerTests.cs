using ContosoUniversity.Data.Entities;
using ContosoUniversity.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ContosoUniversity.Web.ViewModels;
using ContosoUniversity.Web.Enums;
using ContosoUniversity.Common;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class ManageControllerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly FakeUserManager _fakeUserManager;
        private readonly FakeSignInManager _fakeSignInManager;
        ManageController _sut;
        private Mock<ISmsSender> _mockSmsSender;

        public ManageControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _fakeUserManager = new FakeUserManager();
            _fakeSignInManager = new FakeSignInManager();
            _mockSmsSender = new Mock<ISmsSender>();
            _sut = new ManageController(_fakeUserManager, _fakeSignInManager, _mockSmsSender.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult()
        {
            InitTestContext();

            var result = await _sut.Index();

            Assert.IsType<ViewResult>(result);

            var model = (ManageIndexViewModel)((ViewResult)result).Model;
            Assert.True(model.HasPassword);
            Assert.True(model.TwoFactor);
        }

        [Fact]
        public void ChangePassword_ReturnAViewResult()
        {
            var result = _sut.ChangePassword();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ChangePasswordPost_ReturnAViewResult_WithInvalidModel()
        {
            var model = new ChangePasswordViewModel
            {
                OldPassword = "abc",
                NewPassword = "bcd",
                ConfirmPassword = "bcd"
            };
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");

            var result = await _sut.ChangePassword(model);

            Assert.IsType<ViewResult>(result);
            var modelState = ((ViewResult)result).ViewData.ModelState;
            Assert.True(modelState.ContainsKey("mymodelerror"));
        }

        [Fact]
        public async Task ChangePasswordPost_ReturnARedirectToActionResult_WithChangePasswordSuccess()
        {
            var model = new ChangePasswordViewModel
            {
                OldPassword = "abc",
                NewPassword = "bcd",
                ConfirmPassword = "bcd"
            };
            InitTestContext();

            var result = await _sut.ChangePassword(model);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
            Assert.Equal(ManageMessage.ChangePasswordSuccess, ((RedirectToActionResult)result).RouteValues["Message"]);
        }

        [Fact]
        public void AddPhoneNumber_ReturnsAViewResult()
        {
            var result = _sut.AddPhoneNumber();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddPhoneNumberPost_ReturnsAViewResult_WithInvalidModel()
        {
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");

            var result = await _sut.AddPhoneNumber(new AddPhoneNumberViewModel());

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("mymodelerror"));
        }

        [Fact]
        public async Task AddPhoneNumberPost_ReturnsARedirectToAction_VerifyPhoneNumber()
        {
            InitTestContext();
            _mockSmsSender.Setup(m => m.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(0));

            var result = await _sut.AddPhoneNumber(new AddPhoneNumberViewModel());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(((RedirectToActionResult)result).RouteValues.ContainsKey("PhoneNumber"));
        }

        [Fact]
        public async Task VerifyPhoneNumber_ReturnsAViewResult()
        {
            InitTestContext();
            var phoneNumber = "555-555-0123";

            var result = await _sut.VerifyPhoneNumber(phoneNumber);

            Assert.IsType<ViewResult>(result);
            var model = (VerifyPhoneNumberViewModel)((ViewResult)result).Model;
            Assert.Equal(phoneNumber, model.PhoneNumber);
        }

        [Fact]
        public async Task VerifyPhoneNumberPost_ReturnsARedirectToAction_Index()
        {
            InitTestContext();
            var model = new VerifyPhoneNumberViewModel { PhoneNumber = "555-555-0123" };

            var result = await _sut.VerifyPhoneNumber(model);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(ManageMessage.AddPhoneSuccess, ((RedirectToActionResult)result).RouteValues["Message"]);

        }

        [Fact]
        public async Task VerifyPhoneNumberPost_ReturnsAViewResult_WithInvalidModel()
        {
            var model = new VerifyPhoneNumberViewModel { PhoneNumber = "555-555-0123" };
            _sut.ModelState.AddModelError("mymodelerror", "my model error message");

            var result = await _sut.VerifyPhoneNumber(model);

            Assert.IsType<ViewResult>(result);
            Assert.True(((ViewResult)result).ViewData.ModelState.ContainsKey("mymodelerror"));

        }

        [Fact]
        public async Task RemovePhoneNumber_ReturnsAReidrectToActionResult()
        {
            InitTestContext();

            var result = await _sut.RemovePhoneNumber();

            Assert.IsType<RedirectToActionResult>(result);
            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
            Assert.Equal(ManageMessage.RemovePhoneSuccess, ((RedirectToActionResult)result).RouteValues["Message"]);
        }

        [Fact]
        public async Task EnableTwoFactorAuthentication_ReturnsARedirectToActionResult_ToIndex()
        {
            InitTestContext();

            var result = await _sut.EnableTwoFactorAuthentication();

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

        [Fact]
        public async Task DisableTwoFactorAuthentication_ReturnsARedirectToActionResult_ToIndex()
        {
            InitTestContext();

            var result = await _sut.DisableTwoFactorAuthentication();

            Assert.IsType<RedirectToActionResult>(result);

            var actionName = ((RedirectToActionResult)result).ActionName;
            Assert.Equal("Index", actionName);
        }

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

            public override Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
            {
                return Task.FromResult(new ApplicationUser { UserName = "test@example.com" }); // base.GetUserAsync(principal);
            }

            public override Task<bool> HasPasswordAsync(ApplicationUser user)
            {
                return Task.FromResult(true);
            }

            public override Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            public override Task<string> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber)
            {
                return Task.FromResult("change-phone-number-token");
            }
            public override Task<IdentityResult> ChangePhoneNumberAsync(ApplicationUser user, string phoneNumber, string token)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            public override Task<string> GetPhoneNumberAsync(ApplicationUser user)
            {
                return Task.FromResult("224-555-0123");
            }
            public override Task<IdentityResult> SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            public override Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
            {
                return Task.FromResult(true);
            }
            public override Task<IdentityResult> SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
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
                      null,
                      null)
            { }

            public override Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
            {
                return Task.FromResult(0);
            }
        }

        private void InitTestContext()
        {
            var context = new Mock<HttpContext>();
            _sut.ControllerContext = new ControllerContext { };
            _sut.ControllerContext.HttpContext = new DefaultHttpContext { };
        }
    }
}
