using System;
using Microsoft.AspNetCore.Identity;

namespace App.Domain
{
    public class UserToken : IdentityUserToken<int>
    {
        public User User { get; set; }

        public DateTime GeneratedOn { get; set; }   
    }
}