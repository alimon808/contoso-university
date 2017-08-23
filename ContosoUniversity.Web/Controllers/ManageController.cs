using ContosoUniversity.Identity;
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

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ManageMessage? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessage.ChangePasswordSuccess ? "Your password has been changed."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var model = new ManageIndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
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

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        
    }
}