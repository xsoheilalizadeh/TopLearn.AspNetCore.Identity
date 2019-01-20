using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using App.DTOs.Manager.User;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    [Route("user-manager")]
    public class UserManagerController : Controller
    {
        private readonly AppUserManager _userManager;
        private readonly AppRoleManager _roleManager;
        private readonly AppSignInManager _signInManager;

        private readonly ApplicationDbContext _dbContext;

        public UserManagerController(AppUserManager userManager, AppRoleManager roleManager,
            ApplicationDbContext dbContext, 
            AppSignInManager signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
        }

        [HttpGet("", Name = "GetUsers")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }

        [HttpGet("{userName}/roles", Name = "GetUserRoles")]
        [Display(Name = "نقش کاربر")]
        public async Task<IActionResult> UserRoles(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return View(roles);
        }

        [HttpGet("{userName}/logins", Name = "GetUserLogins")]
        public async Task<IActionResult> UserLogins(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var logins = await _userManager.GetLoginsAsync(user);

            return View(logins);
        }

        [HttpGet("{userName}/claims", Name = "GetUserClaims")]
        public async Task<IActionResult> UserClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);

            return View(claims);
        }

        [HttpGet("{userName}/add-role", Name = "GetAddUserRole")]
        public async Task<IActionResult> AddRole(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();

            var roleItems = roles.Select(role => new SelectListItem
            {
                Text = role.DisplayName,
                Value = role.Name
            }).ToList();

            return View(new AddUserRole
            {
                Roles = roleItems,
                UserName = userName,
            });
        }

        [HttpPost("add-role", Name = "PostAddUserRole")]
        public async Task<IActionResult> AddRole(AddUserRole model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return BadRequest();
            }

            var role = await _roleManager.FindByNameAsync(model.SelectedRole);

            if (role == null)
            {
                return BadRequest();
            }

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                return RedirectToRoute("GetUserRoles", new {userName = user.UserName});
            }

            AddErrors(result);

            return View(model);
        }

        [HttpGet("{userName}/add-claim", Name = "GetAddUserClaim")]
        public async Task<IActionResult> AddClaim(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            return View(new AddUserClaim
            {
                UserName = userName
            });
        }

        [HttpPost("add-claim", Name = "PostAddUserClaim")]
        public async Task<IActionResult> AddClaim(AddUserClaim model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return BadRequest();
            }

            var claim = new Claim(model.ClaimType, model.ClaimValue);

            var result = await _userManager.AddClaimAsync(user, claim);

            if (result.Succeeded)
            {
                return RedirectToRoute("GetUserClaims", new {userName = user.UserName});
            }

            AddErrors(result);

            return View(model);
        }

        [HttpGet("{userName}/edit", Name = "GetUserEdit")]
        public async Task<IActionResult> Edit(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest();
            }

            return View(new EditUser
            {
                UserName = userName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age
            });
        }

        [HttpPost("edit", Name = "PostEditUser")]
        public async Task<IActionResult> Edit(EditUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return BadRequest();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Age = model.Age;

            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("GetUsers");
        }


        [HttpPost("remove", Name = "PostUserRemove")]
        public async Task<IActionResult> Remove(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("GetUsers");
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