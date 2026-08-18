using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.Identity;

namespace ShipmentTracker.Infrastructure.Data.Seed
{
    /// <summary>
    /// Seed idempotente de los 5 roles de Identity y de la cuenta SuperAdmin inicial
    /// (research.md Decisión 14). Las credenciales de SuperAdmin vienen de configuración
    /// (Seed:SuperAdminEmail / Seed:SuperAdminPassword), nunca hardcodeadas.
    /// </summary>
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            foreach (var roleName in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }

            var superAdminEmail = configuration["Seed:SuperAdminEmail"];
            var superAdminPassword = configuration["Seed:SuperAdminPassword"];

            if (string.IsNullOrWhiteSpace(superAdminEmail) || string.IsNullOrWhiteSpace(superAdminPassword))
            {
                return;
            }

            var existingSuperAdmin = await userManager.FindByEmailAsync(superAdminEmail);
            if (existingSuperAdmin != null)
            {
                return;
            }

            var superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                EmployeeId = null
            };

            var createResult = await userManager.CreateAsync(superAdmin, superAdminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
            }
        }
    }
}
