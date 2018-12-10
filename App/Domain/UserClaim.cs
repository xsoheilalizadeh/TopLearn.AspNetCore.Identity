using Microsoft.AspNetCore.Identity;

namespace App.Domain
{
    public class UserClaim : IdentityUserClaim<int>
    {
        public User User { get; set; }  
    }
}