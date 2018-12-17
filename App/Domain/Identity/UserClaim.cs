using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity
{
    public class UserClaim : IdentityUserClaim<int>
    {
        public User User { get; set; }  
    }
}