using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.ShipmentEvents
{
    /// <summary>
    /// Detalle de un intento de entrega, anidado dentro de ShipmentEventDto/TrackingEventDto.
    /// </summary>
    public class DeliveryAttemptDetailDto
    {
        /// <summary>
        /// Número de intento para el Shipment asociado.
        /// </summary>
        public int AttemptNumber { get; set; }

        /// <summary>
        /// Motivo por el que no se logró la entrega.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeliveryFailureReason FailureReason { get; set; }

        /// <summary>
        /// Fecha y hora programada para el siguiente intento, si se estableció.
        /// </summary>
        public DateTime? NextAttemptAt { get; set; }
    }
}
