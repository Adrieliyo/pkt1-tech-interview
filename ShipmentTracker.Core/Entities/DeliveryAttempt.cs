using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Core.Entities
{
    /// <summary>
    /// Registro de un intento de entrega fallido. Existe únicamente asociado a un ShipmentEvent de
    /// tipo DeliveryAttempted (uno a uno) — nunca se crea de forma independiente.
    /// </summary>
    public class DeliveryAttempt
    {
        /// <summary>
        /// Identificador único del intento.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador del ShipmentEvent asociado (único — relación uno a uno).
        /// </summary>
        public int ShipmentEventId { get; set; }

        /// <summary>
        /// Navegación hacia el ShipmentEvent (solo de ida; ShipmentEvent no expone colección inversa).
        /// </summary>
        public ShipmentEvent ShipmentEvent { get; set; } = null!;

        /// <summary>
        /// Número de intento, calculado por el servicio como la cuenta de intentos previos para el
        /// mismo Shipment más uno. Nunca provisto por quien llama.
        /// </summary>
        public int AttemptNumber { get; set; }

        /// <summary>
        /// Motivo por el que no se logró la entrega.
        /// </summary>
        public DeliveryFailureReason FailureReason { get; set; }

        /// <summary>
        /// Fecha y hora programada para el siguiente intento, opcional. Cuando se provee, debe ser
        /// posterior a OccurredAt del ShipmentEvent asociado.
        /// </summary>
        public DateTime? NextAttemptAt { get; set; }
    }
}
