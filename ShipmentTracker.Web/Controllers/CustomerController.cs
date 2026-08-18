using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.Customers;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones relacionadas con los clientes (Customers &amp; Accounts).
    /// Escritura restringida a BranchManager — Operator solo tiene lectura (spec 008 FR-013).
    /// </summary>
    [Route("api/customers")]
    [ApiController]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Crea un nuevo cliente Individual.
        /// </summary>
        /// <param name="dto">Datos del cliente Individual a crear.</param>
        /// <returns>El cliente recién creado.</returns>
        [HttpPost("individual")]
        [Authorize(Roles = Roles.BranchManager)]
        public async Task<ActionResult<CustomerDetailDto>> CreateIndividualCustomer([FromBody] CreateIndividualCustomerDto dto)
        {
            try
            {
                var result = await _customerService.CreateIndividualAsync(dto);

                // Se usa Created(uri, body) en vez de CreatedAtAction(nameof(...)) para no
                // depender de la acción GetCustomerById, que se agrega recién en la Historia 2.
                return Created($"/api/customers/{result.Id}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Crea un nuevo cliente Business.
        /// </summary>
        /// <param name="dto">Datos del cliente Business a crear.</param>
        /// <returns>El cliente recién creado.</returns>
        [HttpPost("business")]
        [Authorize(Roles = Roles.BranchManager)]
        public async Task<ActionResult<CustomerDetailDto>> CreateBusinessCustomer([FromBody] CreateBusinessCustomerDto dto)
        {
            try
            {
                var result = await _customerService.CreateBusinessAsync(dto);

                return Created($"/api/customers/{result.Id}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista clientes de forma paginada. Por defecto solo devuelve clientes activos; permite
        /// filtrar por estado activo/inactivo y por tipo de cliente, de forma combinada. La
        /// metadata de paginación se expone en los encabezados <c>X-Total-Count</c>, <c>X-Page</c>,
        /// <c>X-Page-Size</c> y <c>X-Total-Pages</c>.
        /// </summary>
        /// <param name="onlyActive">Si es <c>true</c> (por defecto), devuelve solo clientes activos; si es <c>false</c>, devuelve solo inactivos.</param>
        /// <param name="type">Tipo de cliente opcional para filtrar los resultados.</param>
        /// <param name="page">Número de página solicitada (1-based). Por defecto, 1.</param>
        /// <param name="pageSize">Cantidad de clientes por página. Por defecto, 5. Se recorta a un máximo de 50.</param>
        /// <returns>Una lista de clientes correspondiente a la página solicitada.</returns>
        [HttpGet]
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Operator)]
        public async Task<ActionResult<IEnumerable<CustomerDetailDto>>> GetCustomers(
            [FromQuery] bool onlyActive = true,
            [FromQuery] CustomerType? type = null,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)
        {
            var result = await _customerService.GetCustomersAsync(onlyActive, type, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result.Items);
        }

        /// <summary>
        /// Obtiene los detalles de un cliente por su identificador (activo o inactivo), incluido su detalle específico de tipo.
        /// </summary>
        /// <param name="id">El identificador único del cliente.</param>
        /// <returns>Los detalles del cliente encontrado.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = Roles.BranchManager + "," + Roles.Operator)]
        public async Task<ActionResult<CustomerDetailDto>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound(new { message = $"No customer was found with id '{id}'." });
            }

            return Ok(customer);
        }

        /// <summary>
        /// Reemplaza por completo los datos editables de un cliente existente. El tipo (Individual
        /// o Business) nunca cambia. Puede usarse también para reactivar un cliente inactivo
        /// enviando <c>isActive: true</c> — no existe una acción "activate" separada.
        /// </summary>
        /// <param name="id">El identificador único del cliente.</param>
        /// <param name="dto">Datos completos de reemplazo del cliente.</param>
        /// <returns>El cliente actualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.BranchManager)]
        public async Task<ActionResult<CustomerDetailDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            try
            {
                var result = await _customerService.UpdateCustomerAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No customer was found with id '{id}'." });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Desactiva un cliente (soft-delete: <c>isActive = false</c>). Este es el único mecanismo
        /// para retirar un cliente — nunca se elimina el registro. Idempotente: repetir la
        /// operación sobre un cliente ya inactivo no produce error.
        /// </summary>
        /// <param name="id">El identificador único del cliente.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BranchManager)]
        public async Task<IActionResult> DeactivateCustomer(int id)
        {
            var success = await _customerService.DeactivateCustomerAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"No customer was found with id '{id}'." });
            }

            return NoContent();
        }
    }
}
