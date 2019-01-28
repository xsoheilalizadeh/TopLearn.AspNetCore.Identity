using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace App.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly AppSignInManager _appSignInManager;

        private readonly AppUserManager _appUserManger;

        private readonly ApplicationDbContext _dbContext;

        private readonly IConfiguration _configuration;

        public AuthController(
            IConfiguration configuration,
            AppSignInManager appSignInManager,
            AppUserManager appUserManger, ApplicationDbContext dbContext)
        {
            _appSignInManager = appSignInManager;
            _configuration = configuration;
            _appUserManger = appUserManger;
            _dbContext = dbContext;
        }

        [HttpPost("sign-in")]
        public async Task<object> SignIn(LoginAccount model)
        {
            var user = await _appUserManger.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return new {message = "invalid information"};
            }

            var result = await _appSignInManager.CheckPasswordSignInAsync(user,model.Password,false);

            if (result.Succeeded)
            {
                return new {token = GetToken(user)};
            }

            return new {message = "invalid information"};
        }

        private string GetToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roleClaims = _dbContext.UserRoles.Where(x => x.UserId == user.Id)
                .Include(x => x.Role.Claims)
                .SelectMany(x => x.Role.Claims)
                .ToList();

            foreach (var roleClaim in roleClaims)
            {
                claims.Add(new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
            }

            var key = _configuration["JwtConfig:Key"];
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];

            var expireTime = DateTime.Now.AddDays(4);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expireTime,
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet("secure")]
        [Authorize(nameof(ConstantPolicies.DynamicPermission), AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public string Secure()
        {
            return "Have permission";
        }
    }
}