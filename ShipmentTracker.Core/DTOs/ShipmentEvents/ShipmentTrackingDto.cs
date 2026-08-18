using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.ShipmentEvents
{
    /// <summary>
    /// Respuesta del endpoint público de tracking: resumen del Shipment más su línea de tiempo de
    /// eventos, sin ningún dato de empleado.
    /// </summary>
    public class ShipmentTrackingDto
    {
        /// <summary>
        /// Número de guía único del Shipment.
        /// </summary>
        public string TrackingNumber { get; set; } = null!;

        /// <summary>
        /// Estado actual del Shipment.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShipmentStatus Status { get; set; }

        /// <summary>
        /// Destinatario del Shipment.
        /// </summary>
        public string Recipient { get; set; } = null!;

        /// <summary>
        /// Fecha de creación del Shipment.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de entrega, si ya se entregó.
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// Línea de tiempo de eventos públicos, ordenada cronológicamente. Vacía si aún no hay eventos.
        /// </summary>
        public List<TrackingEventDto> Events { get; set; } = new List<TrackingEventDto>();
    }
}
