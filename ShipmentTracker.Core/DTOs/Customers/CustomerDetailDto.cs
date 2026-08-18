using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.Customers
{
    /// <summary>
    /// Modelo de datos para representar un cliente en la capa de transferencia de datos (DTO).
    /// Union discriminada: exactamente uno de <see cref="Individual"/>/<see cref="Business"/> viene
    /// poblado, según <see cref="Type"/>.
    /// </summary>
    public class CustomerDetailDto
    {
        /// <summary>
        /// Identificador único del cliente.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tipo de cliente (Individual o Business). Fijo desde la creación.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CustomerType Type { get; set; }
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
        /// Indica si el cliente está activo. Los clientes nunca se eliminan, solo se desactivan.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Fecha de creación del registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Fecha de la última actualización. Null si nunca se ha actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Detalle específico de Individual. Poblado solo si <see cref="Type"/> es Individual.
        /// </summary>
        public IndividualDetailDto? Individual { get; set; }
        /// <summary>
        /// Detalle específico de Business. Poblado solo si <see cref="Type"/> es Business.
        /// </summary>
        public BusinessDetailDto? Business { get; set; }
    }
}
