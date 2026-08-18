using ShipmentTracker.Core.DTOs.ShipmentEvents;

namespace ShipmentTracker.Core.Interfaces.Services
{
    /// <summary>
    /// Servicio de dominio para el registro y consulta de eventos de Shipment, incluidos los
    /// intentos de entrega y la vista pública de tracking.
    /// </summary>
    public interface IShipmentEventService
    {
        /// <summary>
        /// Registra un evento genérico (actualmente solo OutForDelivery es aceptado). Devuelve null
        /// si el Shipment no existe; lanza ValidationException por reglas estructurales/de negocio.
        /// </summary>
        Task<ShipmentEventDto?> RegisterEventAsync(int shipmentId, RegisterEventDto dto);

        /// <summary>
        /// Registra un intento de entrega fallido, creando su ShipmentEvent y su DeliveryAttempt
        /// asociado de forma atómica. Devuelve null si el Shipment no existe.
        /// </summary>
        Task<ShipmentEventDto?> RegisterDeliveryAttemptAsync(int shipmentId, RegisterDeliveryAttemptDto dto);

        /// <summary>
        /// Lista todos los eventos de un Shipment (vista operacional, incluye EmployeeId), ordenados
        /// cronológicamente. Devuelve null si el Shipment no existe.
        /// </summary>
        Task<IEnumerable<ShipmentEventDto>?> GetEventsByShipmentAsync(int shipmentId);

        /// <summary>
        /// Obtiene la información pública de tracking de un Shipment por su número de guía. Devuelve
        /// null si no existe.
        /// </summary>
        Task<ShipmentTrackingDto?> GetTrackingAsync(string trackingNumber);
    }
}
