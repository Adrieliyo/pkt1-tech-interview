using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.Shipments;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones relacionadas con los envíos. Lectura
    /// abierta a BranchManager/Operator/WarehouseStaff; creación directa y cambio manual de estado
    /// restringidos a BranchManager por defecto de mínimo privilegio, al no estar nombrados para
    /// ningún otro rol (spec 008 FR-017, research.md Decisión 8).
    /// </summary>
    [Route("api/shipment")]
    [ApiController]
    [Produces("application/json")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        /// <summary>
        /// Obtiene la lista de envíos de forma paginada. Opcionalmente filtra por estado y/o por la
        /// Order que generó el envío. Los envíos se devuelven ordenados por fecha de creación
        /// descendente (más recientes primero). La metadata de paginación (total de registros, página
        /// actual, tamaño de página y total de páginas) se expone en los encabezados
        /// <c>X-Total-Count</c>, <c>X-Page</c>, <c>X-Page-Size</c> y <c>X-Total-Pages</c>.
        /// </summary>
        /// <param name="status">Estado opcional para filtrar los envíos.</param>
        /// <param name="orderId">Identificador opcional de la Order que generó el envío.</param>
        /// <param name="page">Número de página solicitado (1-based). Por defecto, 1.</param>
        /// <param name="pageSize">Cantidad de envíos por página. Por defecto, 5. Se recorta a un máximo de 50.</param>
        /// <returns>Una lista de envíos correspondiente a la página solicitada.</returns>
        [HttpGet]
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Operator + "," + Roles.WarehouseStaff)]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipments(
            [FromQuery] ShipmentStatus? status,
            [FromQuery] int? orderId = null,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)
        {
            var result = await _shipmentService.GetShipmentsAsync(status, orderId, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result.Items); // Devuelve HTTP 200
        }

        /// <summary>
        /// Obtiene los detalles de un envío buscando por número de guía.
        /// </summary>
        /// <param name="trackingNumber">El número de guía único del envío.</param>
        /// <returns>Los detalles del envío encontrado.</returns>
        [HttpGet("{trackingNumber}")]
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Operator + "," + Roles.WarehouseStaff)]
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
        [Authorize(Roles = Roles.BranchManager)]
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
        [Authorize(Roles = Roles.BranchManager)]
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
