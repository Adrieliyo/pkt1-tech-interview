using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using ShipmentTracker.Core.Interfaces.Services;
using System.Security.Claims;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado del registro y consulta de eventos de Shipment, incluidos los
    /// intentos de entrega y la vista pública de tracking. WarehouseStaff tiene solo lectura sobre
    /// los eventos (GetEventsByShipment) — no puede registrar eventos, exclusivo de
    /// BranchManager/Operator/Driver. GetEventsByShipment aplica además un gate a nivel de Service
    /// según el EmployeeId del caller (module 008, research.md Decisión 4): restricción de
    /// asignación para Driver.
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
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Operator + "," + Roles.Driver)]
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
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Driver)]
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
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Operator + "," + Roles.Driver + "," + Roles.WarehouseStaff)]
        public async Task<ActionResult<IEnumerable<ShipmentEventDto>>> GetEventsByShipment(int id)
        {
            try
            {
                var events = await _shipmentEventService.GetEventsByShipmentAsync(id, GetCallerRole(), GetCallerEmployeeId());

                if (events == null)
                {
                    return NotFound(new { message = $"No shipment was found with id '{id}'." });
                }

                return Ok(events);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la información pública de tracking de un Shipment por su número de guía: resumen
        /// del envío más su línea de tiempo de eventos. Endpoint PÚBLICO — nunca incluye
        /// <c>employeeId</c> ni ningún otro dato personal de empleados. Único endpoint, junto con
        /// <c>POST /api/auth/login</c>, explícitamente exento de autenticación en todo el sistema
        /// (module 008, spec FR-016) — [AllowAnonymous] es necesario aquí porque el resto de la API
        /// ahora deniega por defecto (FallbackPolicy en Program.cs).
        /// </summary>
        /// <param name="trackingNumber">El número de guía del Shipment.</param>
        /// <returns>El resumen público de tracking del Shipment.</returns>
        [HttpGet("tracking/{trackingNumber}")]
        [AllowAnonymous]
        public async Task<ActionResult<ShipmentTrackingDto>> GetTracking(string trackingNumber)
        {
            var tracking = await _shipmentEventService.GetTrackingAsync(trackingNumber);

            if (tracking == null)
            {
                return NotFound(new { message = $"No shipment was found with tracking number '{trackingNumber}'." });
            }

            return Ok(tracking);
        }

        // Services no depende de ASP.NET Core (research.md Decisión 9): el Controller extrae el
        // rol/EmployeeId del caller desde los claims de la cookie y los pasa como parámetros planos.
        private string? GetCallerRole() => User.FindFirst(ClaimTypes.Role)?.Value;

        private int? GetCallerEmployeeId() =>
            int.TryParse(User.FindFirstValue("EmployeeId"), out var employeeId) ? employeeId : null;
    }
}
