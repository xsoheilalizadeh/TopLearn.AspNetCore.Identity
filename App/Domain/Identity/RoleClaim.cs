using System;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity
{
    public class RoleClaim : IdentityRoleClaim<int>
    {
        public DateTime GivenOn { get; set; }

        public Role Role { get; set; }
    }
}   