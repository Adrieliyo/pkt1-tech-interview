using System;

namespace ShipmentTracker.Core.DTOs.Customers
{
    /// <summary>
    /// Detalle específico de un cliente de tipo Individual, anidado dentro de <see cref="CustomerDetailDto"/>.
    /// </summary>
    public class IndividualDetailDto
    {
        /// <summary>
        /// Nombre(s) del cliente.
        /// </summary>
        public string FirstName { get; set; } = null!;
        /// <summary>
        /// Apellido(s) del cliente.
        /// </summary>
        public string LastName { get; set; } = null!;
        /// <summary>
        /// Fecha de nacimiento, opcional.
        /// </summary>
        public DateOnly? BirthDate { get; set; }
        /// <summary>
        /// Identificador gubernamental (CURP u equivalente).
        /// </summary>
        public string GovernmentId { get; set; } = null!;
    }
}
