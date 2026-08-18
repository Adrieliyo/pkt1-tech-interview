namespace ShipmentTracker.Core.DTOs.Auth
{
    /// <summary>
    /// Información de la sesión actual: identidad, rol y datos del Employee vinculado (ausentes
    /// para SuperAdmin, que no tiene Employee).
    /// </summary>
    public class UserSessionDto
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>Null para SuperAdmin.</summary>
        public int? EmployeeId { get; set; }

        /// <summary>Null para SuperAdmin.</summary>
        public string? FullName { get; set; }

        /// <summary>El único rol de Identity asignado a la cuenta.</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Null para SuperAdmin.</summary>
        public int? BranchId { get; set; }
    }
}
