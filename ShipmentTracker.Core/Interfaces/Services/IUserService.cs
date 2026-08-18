using ShipmentTracker.Core.DTOs.Auth;

namespace ShipmentTracker.Core.Interfaces.Services
{
    /// <summary>
    /// Servicio de dominio para el aprovisionamiento de cuentas de staff (module 008, US4).
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Crea una cuenta de acceso para un Employee existente y activo que aún no tiene una,
        /// asignando el rol de Identity correspondiente a su EmployeeRole. Lanza
        /// InvalidOperationException si el Employee no existe/no está activo/ya tiene cuenta; lanza
        /// ValidationException por reglas estructurales de la contraseña.
        /// </summary>
        Task<UserSessionDto> CreateUserForEmployeeAsync(CreateUserDto dto);
    }
}
