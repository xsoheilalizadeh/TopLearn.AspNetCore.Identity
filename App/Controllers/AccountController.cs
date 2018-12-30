using System.Linq;
using System.Threading.Tasks;
using App.Domain.Identity;
using App.DTOs.Account;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Register(string returnTo = null)
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


        [HttpGet("sign-in", Name = "GetLogin")]
        public IActionResult Login(string returnTo = null)
        {
            ViewData["returnTo"] = returnTo;

            return View();
        }

        [HttpPost("sign-in", Name = "PostLogin")]
        public async Task<IActionResult> Login(LoginAccount account, string returnTo)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    userName: account.UserName,
                    password: account.Password,
                    isPersistent: account.RememberMe,
                    lockoutOnFailure: false);


                if (result.Succeeded)
                {
                    return RedirectToLocal(returnTo);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToRoute("GetSendCode", new {returnTo, rememberMe = account.RememberMe});
                }

                if (result.IsLockedOut)
                {
                    // todo create lockout view
                    return View("LockOut");
                }

                if (result.IsNotAllowed)
                {
                    if (_userManager.Options.SignIn.RequireConfirmedPhoneNumber)
                    {
                        if (!await _userManager.IsPhoneNumberConfirmedAsync(new User {UserName = account.UserName}))
                        {
                            ModelState.AddModelError(string.Empty, "شماره تلفن شما تایید نشده است.");

                            return View(account);
                        }
                    }


                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(new User {UserName = account.UserName}))
                        {
                            ModelState.AddModelError(string.Empty, "آدرس اییل شما تایید نشده است.");

                            return View(account);
                        }
                    }
                }
            }

            return View(account);
        }

        [HttpGet("send-code", Name = "GetSendCode")]
        public async Task<IActionResult> SendCode(string returnTo, bool rememberMe)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);


            var providerList = providers.Select(provider => new SelectListItem {Value = provider, Text = provider})
                .ToList();


            return View(new SendCode
            {
                RememberMe = rememberMe,
                ReturnTo = returnTo,
                Providers = providerList
            });
        }


        [HttpPost("send-code", Name = "PostSendCode")]
        public async Task<IActionResult> SendCode(SendCode model, string returnTo)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();


            if (user == null)
            {
                return View("Error");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);

            if (string.IsNullOrEmpty(code))
            {
                return View("Error");
            }


            if (model.SelectedProvider == "Email")
            {
                // todo send code with email
            }
            else if (model.SelectedProvider == "Phone")
            {
                // todo send code with sms
            }


            return RedirectToRoute("GetVerifyCode",
                new
                {
                    returnTo,
                    rememberMe = model.RememberMe,
                    provider = model.SelectedProvider
                });
        }


        [HttpGet("verify-code",Name = "GetVerifyCode")]
        public async Task<IActionResult> VerifyCode(string returnTo,bool rememberMe,string provider)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            return View(new VerifyCode
            {
                RememberMe = rememberMe,
                ReturnTo = returnTo,
                Provider = provider
            });
        }

        [HttpPost("verify-code",Name = "PostVerifyCode")]
        public async Task<IActionResult> VerifyCode(VerifyCode model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result = await _signInManager.TwoFactorSignInAsync(
                provider: model.Provider,
                code: model.Code,
                isPersistent: model.RememberMe,
                rememberClient: model.BrowserRemember
            );


            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnTo);
            }

            if (result.IsLockedOut)
            {
                return View("LockOut");
            }

            ModelState.AddModelError(nameof(model.Code),"کد وارد شده معتبر نمی باشد");

            return View(model);

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