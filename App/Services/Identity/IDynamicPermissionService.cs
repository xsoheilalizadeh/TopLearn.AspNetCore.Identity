using System.Security.Claims;

namespace App.Services.Identity
{
    public interface IDynamicPermissionService
    {
        bool CanAccess(ClaimsPrincipal user, string area, string controller, string action);
    }
}   