using AssistantTrainingCore.Models;
using AssistantTrainingCore.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AssistantTrainingCore.Controllers
{
    //Przykład: https://github.com/elanderson/ASP.NET-Core-Basics/blob/master/ASP.NET%20Core%20Basics/src/Contacts/Views/Account/Login.cshtml
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {

            return View();
        }

        public ActionResult SelectUsers([DataSourceRequest] DataSourceRequest request)
        {
            var users = _userManager.Users.ToList();
            var usersDto = new List<AccountIndexData>();

            foreach (var v in users)
            {
                var inputModel = new AccountIndexData
                {
                    ID = v.Id,
                    UserName = v.UserName,
                    RolesInString = string.Empty
                };

                var roles = _userManager.GetRolesAsync(v).GetAwaiter().GetResult();
                foreach (var r in roles)
                {
                    if (!inputModel.RolesInString.Contains(","))
                    {
                        inputModel.RolesInString = r;
                    }
                    else
                    {
                        inputModel.RolesInString = "," + r;
                    }
                }
                usersDto.Add(inputModel);
            }

            return Json(usersDto.ToDataSourceResult(request));
        }

        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Models.LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //_logger.LogInformation(1, "User logged in.");
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Update()
        {
            var us = _userManager.Users.ToList();
            var user = await _userManager.FindByIdAsync("2000eefd-55cd-4bf7-b509-4266e26b1811");
            var u2 = us.Where(u => u.UserName == "admin").First();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resultCh = await _userManager.ResetPasswordAsync(user, token, "ChangeItAsap123!");
            var result = await _signInManager.PasswordSignInAsync("admin", "ChangeItAsap123!", false, false);

            return View();
        }
    }
}