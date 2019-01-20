using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App.Data;
using App.Domain.Identity;
using App.DTOs;
using App.DTOs.Manager;
using App.Services;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    [Route("role-manager")]
    public class RoleManagerController : Controller
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptor;

        private readonly AppRoleManager _roleManager;
        private readonly AppUserManager _userManager;
        private readonly AppSignInManager _signInManager;

        private readonly ApplicationDbContext _dbContext;

        public RoleManagerController(
            AppRoleManager roleManager,
            AppUserManager userManager,
            ApplicationDbContext dbContext,
            IActionDescriptorCollectionProvider actionDescriptor,
            AppSignInManager signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _actionDescriptor = actionDescriptor;
            _signInManager = signInManager;
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


        [HttpGet("{roleName}/permissions", Name = "GetRolePermissions")]
        public async Task<IActionResult> RolePermissions(string roleName)
        {
            var role = await _roleManager.Roles
                .Include(x => x.Claims)
                .SingleOrDefaultAsync(x => x.Name == roleName);

            if (role == null)
            {
                return NotFound();
            }

            var dynamicActions = this.GetDynamicPermissionActions();

            return View(new RolePermission
            {
                Actions = dynamicActions,
                Role = role
            });
        }

        [HttpPost("permissions", Name = "PostRolePermissions")]
        public async Task<IActionResult> RolePermissions(RolePermission model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.Roles
                .Include(x => x.Claims)
                .SingleOrDefaultAsync(x => x.Id == model.RoleId);

            if (role == null)
            {
                return NotFound();
            }

            var selectedPermissions = model.Keys.ToList();

            var roleClaims = role.Claims
                .Where(x => x.ClaimType == ConstantPolicies.DynamicPermission)
                .Select(x => x.ClaimValue)
                .ToList();


            // add new permissions 
            var newPermissions = selectedPermissions.Except(roleClaims).ToList();
            foreach (var permission in newPermissions)
            {
                role.Claims.Add(new RoleClaim
                {
                    ClaimType = ConstantPolicies.DynamicPermission,
                    ClaimValue = permission,
                    GivenOn = DateTime.Now
                });
            }

            // remove deleted permissions
            var removedPermissions = roleClaims.Except(selectedPermissions).ToList();
            foreach (var permission in removedPermissions)
            {
                var roleClaim = role.Claims
                    .SingleOrDefault(x =>
                        x.ClaimType == ConstantPolicies.DynamicPermission &&
                        x.ClaimValue == permission);

                if (roleClaim != null)
                {
                    role.Claims.Remove(roleClaim);
                }
            }

           var result =  await _roleManager.UpdateAsync(role);

           if (result.Succeeded)
           {
               var users = await _userManager.GetUsersInRoleAsync(role.Name);

               foreach (var user in users)
               {
                   await _userManager.UpdateSecurityStampAsync(user);
                   await _signInManager.RefreshSignInAsync(user);
               }


               return RedirectToRoute("GetRolePermissions", new {roleName = role.Name});
           }

           AddErrors(result);

           return View(model);
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

        private List<ActionDto> GetDynamicPermissionActions()
        {
            var actions = new List<ActionDto>();

            var actionDescriptors = _actionDescriptor.ActionDescriptors.Items;

            foreach (var actionDescriptor in actionDescriptors)
            {
                var descriptor = (ControllerActionDescriptor) actionDescriptor;

                var hasPermission = descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>()?
                                        .Policy == ConstantPolicies.DynamicPermission ||
                                    descriptor.MethodInfo.GetCustomAttribute<AuthorizeAttribute>()?
                                        .Policy == ConstantPolicies.DynamicPermission;

                if (hasPermission)
                {
                    actions.Add(new ActionDto
                    {
                        ActionName = descriptor.ActionName,
                        ControllerName = descriptor.ControllerName,
                        ActionDisplayName = descriptor.MethodInfo.GetCustomAttribute<DisplayAttribute>()?.Name,
                        AreaName = descriptor.MethodInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue
                    });
                }
            }

            return actions;
        }
    }
}