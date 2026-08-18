using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ShipmentTracker.Core.Identity;

namespace ShipmentTracker.Infrastructure.Identity
{
    /// <summary>
    /// Añade el claim "EmployeeId" al principal generado al iniciar sesión, para que esté
    /// disponible en cada request sin una consulta extra a base de datos (research.md Decisión 10).
    /// Ausente para SuperAdmin (EmployeeId nulo).
    /// </summary>
    public class ApplicationUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            if (user.EmployeeId.HasValue && principal.Identity is ClaimsIdentity identity)
            {
                identity.AddClaim(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
            }

            return principal;
        }
    }
}
