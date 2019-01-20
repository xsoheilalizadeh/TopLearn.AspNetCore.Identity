using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace App.Services.Identity.DynamicPermission
{
    public class DynamicPermissionRequirement : IAuthorizationRequirement
    {
    }

    public class DynamicPermissionHandler : AuthorizationHandler<DynamicPermissionRequirement>
    {
        private readonly IDynamicPermissionService _dynamicPermissionService;

        private readonly IHttpContextAccessor _contextAccessor;

        public DynamicPermissionHandler(IDynamicPermissionService dynamicPermissionService,
            IHttpContextAccessor contextAccessor
        )
        {
            _contextAccessor = contextAccessor;
            _dynamicPermissionService = dynamicPermissionService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            DynamicPermissionRequirement requirement)
        {
            var routeData = _contextAccessor.HttpContext.GetRouteData().Values;

            var controller = routeData["controller"].ToString();

            var action = routeData["action"].ToString();

            var area = routeData["area"]?.ToString();

            var user = _contextAccessor.HttpContext.User;

            if (_dynamicPermissionService.CanAccess(user, area, action, controller))
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