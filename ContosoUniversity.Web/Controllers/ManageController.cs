using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common;
using ContosoUniversity.Web.Enums;
using ContosoUniversity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContosoUniversity.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ISmsSender _smsSender;

        public ManageController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _smsSender = smsSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ManageMessage? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessage.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessage.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessage.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessage.Error ? "An error has occurred."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var model = new ManageIndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // generate token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber, $"Your security code is: {code}");

            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessage.RemovePhoneSuccess });
                }
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessage.Error });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // send an sms to verify the phone number
            if (phoneNumber == null)
            {
                return View("Error");
            }

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessage.AddPhoneSuccess });
                }
            }

            // if got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return RedirectToAction(nameof(Index), new { Message = ManageMessage.Error });
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessage.ChangePasswordSuccess });
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction(nameof(Index));
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}