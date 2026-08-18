using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.ShipmentEvents
{
    /// <summary>
    /// Datos requeridos para registrar un evento genérico de Shipment. EventType no puede ser
    /// DeliveryAttempted (endpoint dedicado) ni OrderConverted (solo interno, vía conversión de orden).
    /// </summary>
    public class RegisterEventDto
    {
        /// <summary>
        /// Tipo de evento a registrar. Nullable para distinguir "omitido" de un valor explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShipmentEventType? EventType { get; set; }

        /// <summary>
        /// Identificador opcional del Employee que realizó o registró el evento. Requerido y debe
        /// ser un Driver activo cuando EventType es OutForDelivery.
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// Descripción legible de la ubicación del evento, opcional.
        /// </summary>
        public string? LocationLabel { get; set; }

        /// <summary>
        /// Observaciones de texto libre, opcionales.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Momento real en que ocurrió el evento. No puede ser futuro.
        /// </summary>
        public DateTime OccurredAt { get; set; }
    }
}
