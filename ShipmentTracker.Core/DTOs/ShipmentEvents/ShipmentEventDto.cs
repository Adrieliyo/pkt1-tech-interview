using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.ShipmentEvents
{
    /// <summary>
    /// Vista operacional (interna) de un evento de Shipment — incluye EmployeeId. Usada por los
    /// endpoints de registro y por la lista de eventos por Shipment; NO por el endpoint público de tracking.
    /// </summary>
    public class ShipmentEventDto
    {
        /// <summary>
        /// Identificador único del evento.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador del Shipment al que pertenece el evento.
        /// </summary>
        public int ShipmentId { get; set; }

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
        /// Identificador del Employee que realizó o registró el evento, si aplica.
        /// </summary>
        public int? EmployeeId { get; set; }

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
        /// Momento del sistema en que se insertó el registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Detalle del intento de entrega asociado, poblado solo cuando EventType es DeliveryAttempted.
        /// </summary>
        public DeliveryAttemptDetailDto? DeliveryAttempt { get; set; }
    }
}
