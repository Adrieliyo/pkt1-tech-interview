namespace ShipmentTracker.Core.DTOs.Auth
{
    /// <summary>
    /// Credenciales para iniciar sesión.
    /// </summary>
    public class LoginDto
    {
        /// <summary>Correo electrónico de la cuenta.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Contraseña en texto plano (solo en tránsito, nunca almacenada).</summary>
        public string Password { get; set; } = string.Empty;
    }
}
