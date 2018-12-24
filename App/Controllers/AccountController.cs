using System.Threading.Tasks;
using App.Domain.Identity;
using App.DTOs.Account;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly AppUserManager _userManager;
        private readonly AppSignInManager _signInManager;

        public AccountController(AppUserManager userManager, AppSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet("sign-up", Name = "GetRegister")]
        public IActionResult Register(string returnTo)
        {
            ViewData["returnTo"] = returnTo;

            return View();
        }

        [HttpPost("sign-up", Name = "PostRegister")]
        public async Task<IActionResult> Register(RegisterAccount account, string returnTo)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = account.UserName,
                    Email = account.Email,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    RegisteredOn = account.RegisteredOn
                };


                var result = await _userManager.CreateAsync(user, account.Password);

                if (result.Succeeded)
                {
                    //todo implement email verification

                    await _signInManager.SignInAsync(user, false);

                    return RedirectToLocal(returnTo);
                }

                this.AddErrors(result);
            }


            return View(account);
        }

        private IActionResult RedirectToLocal(string returnTo)
        {
            return Redirect(Url.IsLocalUrl(returnTo) ? returnTo : "/");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}