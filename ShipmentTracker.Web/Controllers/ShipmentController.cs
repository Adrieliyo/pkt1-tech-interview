using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones relacionadas con los envíos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;
        
        public ShipmentController(IShipmentService shipmentService, IMapper mapper)
        {
            _shipmentService = shipmentService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene la lista de envíos. Opcionalmente filtra por estado.
        /// </summary>
        /// <param name="status">Estado opcional para filtrar los envíos.</param>
        /// <returns>Una lista de envíos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipments([FromQuery] ShipmentStatus? status)
        {
            var shipments = await _shipmentService.GetShipmentsAsync(status);
            return Ok(shipments); // Devuelve HTTP 200
        }

        /// <summary>
        /// Obtiene los detalles de un envío buscando por número de guía.
        /// </summary>
        /// <param name="trackingNumber">El número de guía único del envío.</param>
        /// <returns>Los detalles del envío encontrado.</returns>
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

        /// <summary>
        /// Crea un nuevo registro de envío.
        /// </summary>
        /// <param name="createShipmentDto">Modelo con la información necesaria para crear el envío.</param>
        /// <returns>El envío recién creado.</returns>
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

        /// <summary>
        /// Actualiza el estado de un envío específico.
        /// </summary>
        /// <param name="trackingNumber">El número de guía del envío.</param>
        /// <param name="newStatus">El nuevo estado a asignar.</param>
        /// <returns>Un código de estado HTTP 204 sin contenido en caso de éxito.</returns>
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
