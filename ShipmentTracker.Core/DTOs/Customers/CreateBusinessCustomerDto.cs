namespace ShipmentTracker.Core.DTOs.Customers
{
    /// <summary>
    /// Datos requeridos para la creación de un nuevo cliente Business.
    /// </summary>
    public class CreateBusinessCustomerDto
    {
        /// <summary>
        /// Correo electrónico del cliente, único a nivel compañía (ambos tipos).
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Teléfono de contacto.
        /// </summary>
        public string Phone { get; set; } = null!;
        /// <summary>
        /// Línea de calle de la dirección.
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Ciudad de la dirección.
        /// </summary>
        public string City { get; set; } = null!;
        /// <summary>
        /// Estado o provincia de la dirección (texto libre).
        /// </summary>
        public string State { get; set; } = null!;
        /// <summary>
        /// Código postal de la dirección.
        /// </summary>
        public string ZipCode { get; set; } = null!;
        /// <summary>
        /// País de la dirección.
        /// </summary>
        public string Country { get; set; } = null!;
        /// <summary>
        /// Razón social del cliente.
        /// </summary>
        public string BusinessName { get; set; } = null!;
        /// <summary>
        /// RFC (persona moral), único entre clientes Business.
        /// </summary>
        public string TaxId { get; set; } = null!;
        /// <summary>
        /// Nombre del representante legal.
        /// </summary>
        public string LegalRepresentative { get; set; } = null!;
        /// <summary>
        /// Categoría de industria, opcional.
        /// </summary>
        public string? Industry { get; set; }
        /// <summary>
        /// Límite de crédito para cuentas corporativas, opcional.
        /// </summary>
        public decimal? CreditLimit { get; set; }
    }
}
