using Microsoft.AspNetCore.Identity;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Core.Identity
{
    /// <summary>
    /// Cuenta de acceso de un miembro del staff. EmployeeId/Employee son nullable: obligatorios
    /// para los cuatro roles ligados a Employee, ausentes para SuperAdmin.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }

        public Employee? Employee { get; set; }
    }
}
