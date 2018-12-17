using App.Data;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace App.Services.Identity.Stores
{
    public class AppRoleStore : RoleStore<Role,ApplicationDbContext,int,UserRole,RoleClaim>
    {
        public AppRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}