using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace App.Services.Identity
{
    public class Plan18AuthorizationHandler : AuthorizationHandler<Plan18Requirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Plan18Requirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.GivenName,requirement.FirstName))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}