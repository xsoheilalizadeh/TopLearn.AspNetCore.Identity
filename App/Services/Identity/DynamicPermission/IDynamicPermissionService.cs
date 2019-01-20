using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Services.Identity.DynamicPermission
{
    public interface IDynamicPermissionService  
    {
        bool CanAccess(ClaimsPrincipal user, string area, string action, string controller);
    }
}