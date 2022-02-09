using System.Linq;
using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssistantTrainingCore.Controllers
{

    public class AppUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ApplicationDbContext db;

        public AppUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            db = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return View( new ResetPasswordModel { Token = token});
        }

        /// <summary>
        /// 
        /// </summary>
        /// https://code-maze.com/password-reset-aspnet-core-identity/
        /// <param name="resetPasswordModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View("Index");
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if(!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View("Index");
            }
            
            return Redirect(Request.Headers["Host"].ToString());
        }
        
        [HttpGet]
        public async Task<IActionResult> UpdateAll([FromRoute]int temp)
        {
            if (temp == 0)
            {
                var usList = _userManager.Users.ToList();
                foreach (var us in usList)
                {
                    var user = await _userManager.FindByIdAsync(us.Id);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var newPassword = user.UserName + "ChangeItAsap123!";
                    var resultCh = await _userManager.ResetPasswordAsync(user, token, newPassword);
                }
            }
            return Ok();
        }
    }
}