using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.ShipmentEvents
{
    /// <summary>
    /// Datos requeridos para registrar un intento de entrega fallido. EventType se fuerza a
    /// DeliveryAttempted internamente — no es parte de este DTO.
    /// </summary>
    public class RegisterDeliveryAttemptDto
    {
        /// <summary>
        /// Identificador opcional del Employee que realizó o registró el evento (sin requisito de rol).
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
        /// Momento real en que ocurrió el intento. No puede ser futuro.
        /// </summary>
        public DateTime OccurredAt { get; set; }

        /// <summary>
        /// Motivo por el que no se logró la entrega. Nullable para distinguir "omitido" de un valor explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeliveryFailureReason? FailureReason { get; set; }

        /// <summary>
        /// Fecha y hora programada para el siguiente intento, opcional. Debe ser posterior a OccurredAt.
        /// </summary>
        public DateTime? NextAttemptAt { get; set; }
    }
}
