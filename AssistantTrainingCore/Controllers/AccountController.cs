using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssistantTrainingCore.Controllers
{
    //Przykład: https://github.com/elanderson/ASP.NET-Core-Basics/blob/master/ASP.NET%20Core%20Basics/src/Contacts/Views/Account/Login.cshtml
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ApplicationDbContext db;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            db = applicationDbContext;
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

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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

        public ActionResult Edit(string id)
        {
            var roles = db.Roles.ToList();
            var selectedUser = db.Users.FirstOrDefault(x => x.Id.Equals(id));

            var user = new ApplicationUser();
            var appUser = new UserCreateData();
            appUser.Name = selectedUser.UserName;
            appUser.Email = selectedUser.Email;
            //appUser.SelectedId = selectedUser.Roles.First().RoleId;

            appUser.Items = roles.Select(r => new SelectListItem()
            {
                Value = r.Id,
                Text = r.Name
            });
            return View(appUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserCreateData userVM)
        {
            if (!string.IsNullOrWhiteSpace(userVM.Email)  && !string.IsNullOrEmpty(userVM.Name))
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == userVM.Name);
                if (user != null) //chk for dupes
                {
                    //var user = UserManager.FindByName(userVM.Name);
                    user.Email = userVM.Email;
                    user.EmailConfirmed = true;
                    if (!String.IsNullOrEmpty(userVM.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                        var resultCh = await _userManager.ResetPasswordAsync(user, token, userVM.Password);

                        //user.PasswordHash = UserManager.PasswordHasher.HashPassword(userVM.Password);
                    }

                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        var oldRoleName = _userManager.GetRolesAsync(user).Result.First();
                        var newRoleName = db.Roles.SingleOrDefault(predicate: r => r.Id == userVM.SelectedId).Name;

                        if (oldRoleName != newRoleName)
                        {
                            _ = await _userManager.RemoveFromRoleAsync(user, oldRoleName);
                            _ = await _userManager.AddToRoleAsync(user, newRoleName);
                        }

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    //await SignInAsync(user, true);//user is cached until logout so do this to clear cache
                }
            }
            return View(userVM);
        }

        public ActionResult Delete(string id)
        {
            var user = db.Users.Find(id);
            var roles = db.Roles.ToList();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            _ = db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var appUser = CreateUserModel(string.Empty);
            return View(appUser);
        }

        private UserCreateData CreateUserModel(string message)
        {
            var roles = db.Roles.ToList();
            var user = new ApplicationUser();
            var appUser = new UserCreateData();
            appUser.Items = roles.Select(r => new SelectListItem()
            {
                Value = r.Id,
                Text = r.Name
            });
            appUser.Message = message;
            return appUser;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(UserCreateData userVM)
        {
            if (!string.IsNullOrEmpty(userVM.Name) && !string.IsNullOrEmpty(userVM.Password))
            {
                var selectedRole = db.Roles.Where(r => r.Id.Equals(userVM.SelectedId)).FirstOrDefault();

                if (null != selectedRole)
                {
                    if (!db.Users.Any(u => u.UserName == userVM.Name))
                    {
                        var done = await _userManager.CreateAsync(new IdentityUser
                        {
                            UserName = userVM.Name,
                            Email = userVM.Email
                        }, userVM.Password);

                        if (done.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(_userManager.Users.Where(u => u.UserName == userVM.Name).First(), selectedRole.Name);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return View("Create", CreateUserModel(done.Errors.FirstOrDefault()?.Description));
                        }
                    }
                    else
                    {
                        return View("Create", CreateUserModel("User already exist"));
                    }
                }
                return View("Create", CreateUserModel("Select role"));
            }
            return RedirectToAction("Create");
        }
    }
}