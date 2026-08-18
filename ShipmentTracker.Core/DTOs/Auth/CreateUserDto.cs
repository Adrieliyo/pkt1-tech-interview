namespace ShipmentTracker.Core.DTOs.Auth
{
    /// <summary>
    /// Datos para que un SuperAdmin provisione un nuevo login de staff vinculado a un Employee
    /// existente. El rol y el email de la cuenta se derivan del Employee, no se reciben aquí.
    /// </summary>
    public class CreateUserDto
    {
        public int EmployeeId { get; set; }

        public string Password { get; set; } = string.Empty;
    }
}
