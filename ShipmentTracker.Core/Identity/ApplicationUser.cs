using Microsoft.AspNetCore.Identity;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Core.Identity
{
    /// <summary>
    /// Cuenta de acceso de un miembro del staff. EmployeeId/Employee son nullable: obligatorios
    /// para los cuatro roles ligados a Employee, ausentes para SuperAdmin (research.md Decisión 2).
    /// Vive en Core (no en Infrastructure) porque Services necesita construir instancias vía
    /// UserManager en UserService.CreateUserForEmployeeAsync — IdentityUser no tiene dependencia de
    /// EF Core por sí mismo, así que no rompe la regla de "solo Infrastructure toca EF Core"
    /// (research.md Decisión 1, corregida durante la implementación).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }

        public Employee? Employee { get; set; }
    }
}
