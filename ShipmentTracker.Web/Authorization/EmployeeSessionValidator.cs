using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Identity;

namespace ShipmentTracker.Web.Authorization
{
    /// <summary>
    /// Revalida en cada request que el Employee vinculado (si lo hay) siga activo y que su rol de
    /// Identity siga coincidiendo con su EmployeeRole actual.
    /// Registrado como OnValidatePrincipal junto con SecurityStampValidatorOptions.ValidationInterval
    /// = TimeSpan.Zero para que corra en cada request, no cada 30 minutos.
    /// </summary>
    public static class EmployeeSessionValidator
    {
        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            // Registrar este delegado como OnValidatePrincipal REEMPLAZA por completo el
            // SecurityStampValidator que AddIdentity() conecta por defecto — sin resolverlo y
            // llamarlo explícitamente aquí, un logout (que regenera el SecurityStamp, ver
            // AuthController.Logout) nunca se detectaría y la cookie ya "cerrada" seguiría siendo
            // válida hasta su expiración. ValidateAsync ya llama a context.RejectPrincipal()
            // internamente si el stamp no coincide o la cuenta está bloqueada.
            var stampValidator = context.HttpContext.RequestServices.GetRequiredService<ISecurityStampValidator>();
            await stampValidator.ValidateAsync(context);
            if (context.Principal == null)
            {
                return;
            }

            var principal = context.Principal;

            // SuperAdmin no tiene Employee vinculado — nada que revalidar contra ese modelo.
            if (principal.IsInRole(Roles.SuperAdmin))
            {
                return;
            }

            var employeeIdClaim = principal.FindFirstValue("EmployeeId");
            if (!int.TryParse(employeeIdClaim, out var employeeId))
            {
                await RejectAsync(context);
                return;
            }

            var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var employee = await unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.Id == employeeId);

            if (employee == null || !employee.IsActive)
            {
                await RejectAsync(context);
                return;
            }

            var currentRoleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
            var expectedRole = employee.Role.ToString();

            if (currentRoleClaim != expectedRole)
            {
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var user = userId == null ? null : await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    await RejectAsync(context);
                    return;
                }

                var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                var refreshedPrincipal = await signInManager.CreateUserPrincipalAsync(user);
                context.ReplacePrincipal(refreshedPrincipal);
                context.ShouldRenew = true;
            }
        }

        private static async Task RejectAsync(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        }
    }
}
