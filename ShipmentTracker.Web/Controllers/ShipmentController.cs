using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;

namespace ShipmentTracker.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;
        
        public ShipmentController(IShipmentService shipmentService, IMapper mapper)
        {
            _shipmentService = shipmentService;
            _mapper = mapper;
        }

        // GET: api/shipments
        // GET: api/shipments?status=Collected
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipments([FromQuery] ShipmentStatus? status)
        {
            var shipments = await _shipmentService.GetShipmentsAsync(status);
            return Ok(shipments); // Devuelve HTTP 200
        }

        // GET: api/shipments/TRK-A1B2C3D4
        [HttpGet("{trackingNumber}")]
        public async Task<ActionResult<ShipmentDto>> GetShipmentByTrackingNumber(string trackingNumber)
        {
            var shipment = await _shipmentService.GetShipmentByTrackingNumberAsync(trackingNumber);

            if (shipment == null)
            {
                return NotFound(new { message = $"No se encontró un envío con la guía '{trackingNumber}'." }); // HTTP 404
            }

            return Ok(shipment); // HTTP 200
        }

        // POST: api/shipments
        [HttpPost]
        public async Task<ActionResult<ShipmentDto>> CreateShipment([FromBody] CreateShipmentDto createShipmentDto)
        {
            var newShipment = await _shipmentService.CreateShipmentAsync(createShipmentDto);

            // Retorna HTTP 201 (Created) e incluye en los Headers (Location) la URL 
            // exacta para consultar el recurso que se acaba de crear.
            return CreatedAtAction(
                nameof(GetShipmentByTrackingNumber),
                new { trackingNumber = newShipment.TrackingNumber },
                newShipment
            );
        }

        // PATCH: api/shipments/TRK-A1B2C3D4/status
        [HttpPatch("{trackingNumber}/status")]
        public async Task<IActionResult> UpdateStatus(string trackingNumber, [FromBody] ShipmentStatus newStatus)
        {
            try
            {
                var success = await _shipmentService.UpdateShipmentStatusAsync(trackingNumber, newStatus);

                if (!success)
                {
                    return NotFound(new { message = $"No se encontró un envío con la guía '{trackingNumber}'." }); // HTTP 404
                }

                // HTTP 204: La operación fue exitosa, pero no hay contenido que devolver en el body.
                // Es el estándar de REST para actualizaciones (PUT/PATCH).
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Aquí capturamos la excepción que lanzaría nuestro ShipmentTransitionValidator
                // y le devolvemos al cliente un error 400 Bad Request con el mensaje claro.
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
