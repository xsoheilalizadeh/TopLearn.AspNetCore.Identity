using System;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity
{
    public class UserLogin : IdentityUserLogin<int>
    {
        public User User { get; set; }

        public DateTime LoggedOn { get; set; }  
    }
}