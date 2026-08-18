using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ShipmentTracker.Core.Constants;

namespace ShipmentTracker.Web.Authorization
{
    /// <summary>
    /// Un SuperAdmin siempre pasa cualquier [Authorize(Roles = "...")], sin necesidad de listarlo
    /// manualmente en cada controlador. Se registra junto al
    /// RolesAuthorizationHandler propio de ASP.NET Core — un Succeed() de cualquiera de los dos
    /// handlers basta para satisfacer el requirement.
    /// </summary>
    public class SuperAdminAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.IsInRole(Roles.SuperAdmin))
            {
                foreach (var requirement in context.PendingRequirements.OfType<RolesAuthorizationRequirement>().ToList())
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
