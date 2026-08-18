using System;

namespace ShipmentTracker.Core.DTOs.Customers
{
    /// <summary>
    /// Datos requeridos para la creación de un nuevo cliente Individual.
    /// </summary>
    public class CreateIndividualCustomerDto
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
        /// Identificador gubernamental (CURP), único entre clientes Individual.
        /// </summary>
        public string GovernmentId { get; set; } = null!;
    }
}
