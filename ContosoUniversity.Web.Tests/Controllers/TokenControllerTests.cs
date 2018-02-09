using ContosoUniversity.Data.Entities;
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
using ContosoUniversity.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class TokenControllerTests
    {
        TokenController _sut;
        private readonly FakeSignInManager _fakeSignInManager;
        private readonly FakeUserManager _fakeUserManager;

        public TokenControllerTests(ITestOutputHelper output)
        {
            _fakeSignInManager = new FakeSignInManager();
            _fakeUserManager = new FakeUserManager();
            var config = new ConfigurationBuilder()
              .SetBasePath(Path.GetFullPath(@"..\..\..\..\ContosoUniversity.Web"))
              .AddJsonFile("appsettings.development.json")
              .Build();
            
            _sut = new TokenController(_fakeSignInManager, _fakeUserManager, config);
        }

        [Fact(Skip = "Local Test Only")]
        public async Task Create_ReturnsBadRequestAsync()
        {
            var vm = new TokenViewModel();
            _sut.ModelState.AddModelError("myerror", "my errror message");

            var result = await _sut.Create(vm);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact(Skip = "Local Test Only")]
        public async Task Create_ReturnsToken()
        {
            var vm = new TokenViewModel
            {
                Email = "admin@contoso.com",
                Password = "Pass@word1!"
            };
            
            var result = await _sut.Create(vm);

            Assert.IsType<OkObjectResult>(result);

            string token = ((OkObjectResult)result).Value.ToString();
            Assert.Contains("token", token);
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
                return Task.FromResult(new ApplicationUser { Email = email });
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
            public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
            {
                return Task.FromResult(true);
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
