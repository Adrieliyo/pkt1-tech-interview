using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using ShipmentTracker.Core.Interfaces.Services;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado del registro y consulta de eventos de Shipment, incluidos los
    /// intentos de entrega y la vista pública de tracking.
    /// </summary>
    [Route("api/shipments")]
    [ApiController]
    [Produces("application/json")]
    public class ShipmentEventController : ControllerBase
    {
        private readonly IShipmentEventService _shipmentEventService;

        public ShipmentEventController(IShipmentEventService shipmentEventService)
        {
            _shipmentEventService = shipmentEventService;
        }

        /// <summary>
        /// Registra un evento genérico para un Shipment. Actualmente solo se acepta
        /// <c>EventType: "OutForDelivery"</c> — <c>DeliveryAttempted</c> y <c>OrderConverted</c> se
        /// rechazan (tienen su propio flujo de creación dedicado/interno).
        /// </summary>
        /// <param name="id">El identificador único del Shipment.</param>
        /// <param name="dto">Datos del evento a registrar.</param>
        /// <returns>El evento recién creado.</returns>
        [HttpPost("{id}/events")]
        public async Task<ActionResult<ShipmentEventDto>> RegisterEvent(int id, [FromBody] RegisterEventDto dto)
        {
            try
            {
                var result = await _shipmentEventService.RegisterEventAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No shipment was found with id '{id}'." });
                }

                return Created($"/api/shipments/{id}/events", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Registra un intento de entrega fallido para un Shipment que actualmente está
        /// <c>OutForDelivery</c>. Crea automáticamente su DeliveryAttempt asociado (número de intento
        /// calculado por el sistema) sin modificar el estado del Shipment.
        /// </summary>
        /// <param name="id">El identificador único del Shipment.</param>
        /// <param name="dto">Datos del intento de entrega a registrar.</param>
        /// <returns>El evento recién creado, con su detalle de intento de entrega.</returns>
        [HttpPost("{id}/events/delivery-attempt")]
        public async Task<ActionResult<ShipmentEventDto>> RegisterDeliveryAttempt(int id, [FromBody] RegisterDeliveryAttemptDto dto)
        {
            try
            {
                var result = await _shipmentEventService.RegisterDeliveryAttemptAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No shipment was found with id '{id}'." });
                }

                return Created($"/api/shipments/{id}/events", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista todos los eventos registrados para un Shipment, ordenados cronológicamente. Vista
        /// OPERACIONAL (interna): incluye <c>employeeId</c>. No paginado — la cantidad de eventos por
        /// Shipment es pequeña y acotada.
        /// </summary>
        /// <param name="id">El identificador único del Shipment.</param>
        /// <returns>La lista de eventos del Shipment.</returns>
        [HttpGet("{id}/events")]
        public async Task<ActionResult<IEnumerable<ShipmentEventDto>>> GetEventsByShipment(int id)
        {
            var events = await _shipmentEventService.GetEventsByShipmentAsync(id);

            if (events == null)
            {
                return NotFound(new { message = $"No shipment was found with id '{id}'." });
            }

            return Ok(events);
        }

        /// <summary>
        /// Obtiene la información pública de tracking de un Shipment por su número de guía: resumen
        /// del envío más su línea de tiempo de eventos. Endpoint PÚBLICO — nunca incluye
        /// <c>employeeId</c> ni ningún otro dato personal de empleados. No requiere autenticación (este
        /// proyecto no cuenta con middleware de autenticación en ningún endpoint).
        /// </summary>
        /// <param name="trackingNumber">El número de guía del Shipment.</param>
        /// <returns>El resumen público de tracking del Shipment.</returns>
        [HttpGet("tracking/{trackingNumber}")]
        public async Task<ActionResult<ShipmentTrackingDto>> GetTracking(string trackingNumber)
        {
            var tracking = await _shipmentEventService.GetTrackingAsync(trackingNumber);

            if (tracking == null)
            {
                return NotFound(new { message = $"No shipment was found with tracking number '{trackingNumber}'." });
            }

            return Ok(tracking);
        }
    }
}
