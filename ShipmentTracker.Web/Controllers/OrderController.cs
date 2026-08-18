using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar el ciclo de vida de las órdenes de envío:
    /// creación, listado, consulta, actualización, confirmación, cancelación y conversión a Shipment.
    /// </summary>
    [Route("api/orders")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Crea una nueva orden de envío. La orden nace con estado <c>Pending</c> y un número de
        /// orden generado automáticamente (ORD-YYYYMMDD-XXXX).
        /// </summary>
        /// <param name="dto">Datos de la orden a crear.</param>
        /// <returns>La orden recién creada.</returns>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(dto);

                return Created($"/api/orders/{result.Id}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista órdenes de forma paginada, opcionalmente filtradas por customerId y/o status,
        /// ordenadas por fecha de creación descendente (más recientes primero). La metadata de
        /// paginación se expone en los encabezados <c>X-Total-Count</c>, <c>X-Page</c>,
        /// <c>X-Page-Size</c> y <c>X-Total-Pages</c>.
        /// </summary>
        /// <param name="customerId">Identificador del Customer cuyas órdenes se desean (opcional).</param>
        /// <param name="status">Estado de orden por el que filtrar (opcional).</param>
        /// <param name="page">Número de página solicitada (1-based). Por defecto, 1.</param>
        /// <param name="pageSize">Cantidad de órdenes por página. Por defecto, 5. Se recorta a un máximo de 50.</param>
        /// <returns>Una lista de órdenes correspondiente a la página solicitada.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
            [FromQuery] int? customerId = null,
            [FromQuery] OrderStatus? status = null,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)
        {
            var result = await _orderService.GetOrdersAsync(customerId, status, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result.Items);
        }

        /// <summary>
        /// Obtiene los detalles completos de una orden por su identificador, en cualquier estado.
        /// </summary>
        /// <param name="id">El identificador único de la orden.</param>
        /// <returns>Los detalles de la orden encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { message = $"No order was found with id '{id}'." });
            }

            return Ok(order);
        }

        /// <summary>
        /// Obtiene una orden por su número de orden legible por humanos (ORD-YYYYMMDD-XXXX).
        /// </summary>
        /// <param name="orderNumber">El número de orden.</param>
        /// <returns>Los detalles de la orden encontrada.</returns>
        [HttpGet("number/{orderNumber}")]
        public async Task<ActionResult<OrderDto>> GetOrderByNumber(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);

            if (order == null)
            {
                return NotFound(new { message = $"No order was found with number '{orderNumber}'." });
            }

            return Ok(order);
        }

        /// <summary>
        /// Actualiza una orden pendiente (único estado editable), re-validando las mismas reglas de
        /// creación. La propiedad (CustomerId) no es editable.
        /// </summary>
        /// <param name="id">El identificador único de la orden.</param>
        /// <param name="dto">Datos completos de reemplazo de la orden.</param>
        /// <returns>La orden actualizada.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            try
            {
                var result = await _orderService.UpdateOrderAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No order was found with id '{id}'." });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Confirma una orden pendiente (Pending → Confirmed), bloqueándola para ediciones.
        /// </summary>
        /// <param name="id">El identificador único de la orden.</param>
        /// <returns>La orden confirmada.</returns>
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<OrderDto>> ConfirmOrder(int id)
        {
            try
            {
                var result = await _orderService.ConfirmOrderAsync(id);

                if (result == null)
                {
                    return NotFound(new { message = $"No order was found with id '{id}'." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una orden pendiente (Pending → Cancelled). La fila nunca se elimina; el estado
        /// Cancelled es terminal. No es idempotente: repetir sobre una orden ya no pendiente devuelve 400.
        /// </summary>
        /// <param name="id">El identificador único de la orden.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var success = await _orderService.CancelOrderAsync(id);

                if (!success)
                {
                    return NotFound(new { message = $"No order was found with id '{id}'." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Convierte una orden confirmada en un Shipment: genera la guía (TRK-YYYYMMDD-XXXX), crea el
        /// Shipment con su primer ShipmentEvent y marca la orden como Converted, todo en una sola
        /// operación atómica.
        /// </summary>
        /// <param name="id">El identificador único de la orden.</param>
        /// <returns>El identificador y número de guía del Shipment recién creado.</returns>
        [HttpPost("{id}/convert")]
        public async Task<ActionResult<ConvertOrderResultDto>> ConvertOrder(int id)
        {
            try
            {
                var result = await _orderService.ConvertToShipmentAsync(id);

                if (result == null)
                {
                    return NotFound(new { message = $"No order was found with id '{id}'." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}