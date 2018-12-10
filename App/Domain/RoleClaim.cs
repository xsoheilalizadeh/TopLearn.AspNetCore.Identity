using System;
using Microsoft.AspNetCore.Identity;

namespace App.Domain
{
    public class RoleClaim : IdentityRoleClaim<int>
    {
        public DateTime GivenOn { get; set; }

        public Role Role { get; set; }
    }
}   