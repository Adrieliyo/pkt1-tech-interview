namespace ShipmentTracker.Core.DTOs.Customers
{
    /// <summary>
    /// Detalle específico de un cliente de tipo Business, anidado dentro de <see cref="CustomerDetailDto"/>.
    /// </summary>
    public class BusinessDetailDto
    {
        /// <summary>
        /// Razón social del cliente.
        /// </summary>
        public string BusinessName { get; set; } = null!;
        /// <summary>
        /// RFC (persona moral).
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
