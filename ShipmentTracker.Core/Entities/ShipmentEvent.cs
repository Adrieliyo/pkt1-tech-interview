using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Core.Entities
{
    /// <summary>
    /// Registro de algo que le ocurrió a un Shipment a lo largo de su ciclo de vida.
    /// Este módulo lo escribe únicamente en la conversión de una orden (primer evento de tipo OrderConverted);
    /// módulos futuros pueden registrar más tipos de evento sin cambio de esquema (la columna se persiste como string).
    /// </summary>
    public class ShipmentEvent
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
        /// Navegación hacia el Shipment (solo de ida; Shipment no expone colección inversa).
        /// </summary>
        public Shipment Shipment { get; set; } = null!;

        /// <summary>
        /// Tipo de evento (OrderConverted para este módulo).
        /// </summary>
        public ShipmentEventType EventType { get; set; }

        /// <summary>
        /// Instantánea del estado del Shipment al momento del evento, usando el enum ShipmentStatus existente.
        /// </summary>
        public ShipmentStatus StatusSnapshot { get; set; }

        /// <summary>
        /// Momento en el que ocurrió el evento (UTC), asignado por el servicio.
        /// </summary>
        public DateTime OccurredAt { get; set; }
    }
}