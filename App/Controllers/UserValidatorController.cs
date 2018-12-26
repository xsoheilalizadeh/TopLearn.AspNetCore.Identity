using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    [Route("user-validation")]
    public class UserValidatorController : Controller
    {
        private readonly AppUserManager _userManager;

        public UserValidatorController(AppUserManager userManager)
        {
            _userManager = userManager;
        }


        [HttpGet("validate-userName", Name = "ValidateUserName")]
        public async Task<IActionResult> ValidateUserName(string userName)
        {
            var result = await _userManager.Users.AnyAsync(user => user.UserName == userName);

            return Json(!result);
        }


        [HttpGet("validate-email", Name = "ValidateEmail")]
        public async Task<IActionResult> ValidateEmail(string email)
        {
            var result = await _userManager.Users.AnyAsync(user => user.Email == email);

            return Json(!result);
        }
    }
}