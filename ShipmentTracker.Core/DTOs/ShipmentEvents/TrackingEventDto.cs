using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.ShipmentEvents
{
    /// <summary>
    /// Vista pública (segura) de un evento de Shipment — sin Id, sin EmployeeId, sin CreatedAt.
    /// Usada exclusivamente por el endpoint público de tracking.
    /// </summary>
    public class TrackingEventDto
    {
        /// <summary>
        /// Tipo de evento.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShipmentEventType EventType { get; set; }

        /// <summary>
        /// Instantánea del estado del Shipment al momento del evento.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShipmentStatus StatusSnapshot { get; set; }

        /// <summary>
        /// Descripción legible de la ubicación del evento, si se proporcionó.
        /// </summary>
        public string? LocationLabel { get; set; }

        /// <summary>
        /// Observaciones de texto libre, si se proporcionaron.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Momento real en que ocurrió el evento.
        /// </summary>
        public DateTime OccurredAt { get; set; }

        /// <summary>
        /// Detalle del intento de entrega asociado, poblado solo cuando EventType es DeliveryAttempted.
        /// </summary>
        public DeliveryAttemptDetailDto? DeliveryAttempt { get; set; }
    }
}
