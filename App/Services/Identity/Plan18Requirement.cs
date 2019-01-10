using Microsoft.AspNetCore.Authorization;

namespace App.Services.Identity
{
    public class Plan18Requirement : IAuthorizationRequirement
    {
        public Plan18Requirement(string firstName)
        {
            this.FirstName = firstName;
        }

        public string FirstName { get; set; }   
    }
}