using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using App.Authorization;
using App.Data;
using App.Domain.Identity;
using App.DTOs.Account;
using App.Services;
using App.Services.Identity.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly AppUserManager _userManager;

        private readonly AppSignInManager _signInManager;

        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _dbContext;

        public AuthController(
            AppUserManager userManager,
            AppSignInManager signInManager,
            IConfiguration configuration,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost("sign-in")]
        public async Task<object> SignIn(LoginAccount model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return new {message = "Invalid information"};
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var token = await GetTokenAsync(user);

                return new {token};
            }

            return new {message = "Invalid information"};
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("secure")]
        public object Secure()
        {
            return "I have permission";
        }

        
        [JwtAuthorize(Policy = ConstantPolicies.DynamicPermission)]
        [HttpGet("secure-permission")]
        public object SecurePermission()
        {
            return "I have dynamic permission";
        }

        [JwtAuthorize(Roles = "Admin")]
        [HttpGet("secure-role")]
        public object SecureRole()
        {
            return "I have role permission";
        }

        private async Task<string> GetTokenAsync(User user)
        {
            var issuer = _configuration["JwtConfig:Issuer"];

            var audience = _configuration["JwtConfig:Audience"];

            var key = _configuration["JwtConfig:Key"];

            var expires = DateTime.Now.AddDays(5);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var userRoles = await _dbContext.UserRoles
                .Include(userRole => userRole.Role.Claims)
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                foreach (var roleClaim in userRole.Role.Claims)
                {
                    claims.Add(new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
                }
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}