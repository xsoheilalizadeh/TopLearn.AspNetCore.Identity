using System.Threading.Tasks;
using App.Data;
using App.Domain.Identity;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    [Route("role-manager")]
    public class RoleManagerController : Controller
    {
        private readonly AppRoleManager _roleManager;
        private readonly AppUserManager _userManager;

        private readonly ApplicationDbContext _dbContext;

        public RoleManagerController(AppRoleManager roleManager, AppUserManager userManager, ApplicationDbContext dbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet("", Name = "GetRoles")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }

        [HttpGet("{roleName}/users", Name = "GetRoleUsers")]
        public async Task<IActionResult> RoleUsers(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                return NotFound();
            }

            var users = await _userManager.GetUsersInRoleAsync(roleName);

            return View(users);
        }

        [HttpGet("{roleName}/claims", Name = "GetRoleClaims")]
        public async Task<IActionResult> RoleClaims(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                return NotFound();
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            return View(claims);
        }

        [HttpGet("new", Name = "GetCreateRole")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost("new", Name = "PostCreateRole")]
        public async Task<IActionResult> Create(Role model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _roleManager.CreateAsync(model);

            if (result.Succeeded)
            {
                return RedirectToRoute("GetRoles");
            }

            AddErrors(result);

            return View(model);
        }


        [HttpGet("{roleName}/edit", Name = "GetEditRole")]
        public async Task<IActionResult> Edit(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost("edit", Name = "PostEditRole")]
        public async Task<IActionResult> Edit(Role model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _dbContext.Roles.Update(model);

            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("GetRoles");
        }



        [HttpPost("remove", Name = "PostRemoveRole")]
        public async Task<IActionResult> Remove(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            _dbContext.Roles.Remove(role);

            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("GetRoles");
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