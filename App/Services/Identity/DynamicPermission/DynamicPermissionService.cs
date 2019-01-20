using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Services.Identity.DynamicPermission
{
    public class DynamicPermissionService : IDynamicPermissionService
    {
        public bool CanAccess(ClaimsPrincipal user, string area, string action, string controller)
        {
            var key = $"{area}:{controller}:{action}";

            return user.HasClaim("DynamicPermission", key);
        }
    }
}