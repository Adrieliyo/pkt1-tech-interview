using System;

namespace ShipmentTracker.Core.DTOs.Customers
{
    /// <summary>
    /// Datos de reemplazo de un cliente existente. Los campos compartidos son requeridos; los
    /// campos específicos de tipo (Individual y Business) son todos nullable — el servicio decide,
    /// según el tipo real ya persistido del cliente, cuáles son requeridos y cuáles deben venir en
    /// null (no hay campo `Type`: el tipo es inmutable, ver FR-004).
    /// </summary>
    public class UpdateCustomerDto
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
        /// Indica si el cliente está activo. Permite reactivar un cliente inactivo.
        /// </summary>
        public bool IsActive { get; set; }

        // Campos exclusivos de Individual — deben venir null si el cliente es Business.
        /// <summary>
        /// Nombre(s) del cliente. Solo aplica si el cliente es Individual.
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// Apellido(s) del cliente. Solo aplica si el cliente es Individual.
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// Fecha de nacimiento, opcional. Solo aplica si el cliente es Individual.
        /// </summary>
        public DateOnly? BirthDate { get; set; }
        /// <summary>
        /// Identificador gubernamental (CURP). Solo aplica si el cliente es Individual.
        /// </summary>
        public string? GovernmentId { get; set; }

        // Campos exclusivos de Business — deben venir null si el cliente es Individual.
        /// <summary>
        /// Razón social del cliente. Solo aplica si el cliente es Business.
        /// </summary>
        public string? BusinessName { get; set; }
        /// <summary>
        /// RFC (persona moral). Solo aplica si el cliente es Business.
        /// </summary>
        public string? TaxId { get; set; }
        /// <summary>
        /// Nombre del representante legal. Solo aplica si el cliente es Business.
        /// </summary>
        public string? LegalRepresentative { get; set; }
        /// <summary>
        /// Categoría de industria, opcional. Solo aplica si el cliente es Business.
        /// </summary>
        public string? Industry { get; set; }
        /// <summary>
        /// Límite de crédito para cuentas corporativas, opcional. Solo aplica si el cliente es Business.
        /// </summary>
        public decimal? CreditLimit { get; set; }
    }
}
