using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.Vehicles;
using ShipmentTracker.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones relacionadas con los vehículos.
    /// Gestión de Vehicles restringida a BranchManager en su totalidad (spec 008 FR-013/FR-014/FR-015).
    /// </summary>
    [Route("api/vehicles")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = Roles.BranchManager)]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Crea un nuevo vehículo, asignado a una sucursal activa.
        /// </summary>
        /// <param name="dto">Datos del vehículo a crear.</param>
        /// <returns>El vehículo recién creado.</returns>
        [HttpPost]
        public async Task<ActionResult<VehicleDto>> CreateVehicle([FromBody] CreateVehicleDto dto)
        {
            try
            {
                var result = await _vehicleService.CreateVehicleAsync(dto);

                // Se usa Created(uri, body) en vez de CreatedAtAction(nameof(...)) para no
                // depender de la acción GetVehicleById, que se agrega recién en la Historia 4.
                return Created($"/api/vehicles/{result.Id}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista vehículos de forma paginada. Solo devuelve vehículos activos. Permite filtrar
        /// opcionalmente por sucursal. La metadata de paginación se expone en los encabezados
        /// <c>X-Total-Count</c>, <c>X-Page</c>, <c>X-Page-Size</c> y <c>X-Total-Pages</c>.
        /// </summary>
        /// <param name="branchId">Identificador de sucursal opcional para filtrar los resultados.</param>
        /// <param name="page">Número de página solicitada (1-based). Por defecto, 1.</param>
        /// <param name="pageSize">Cantidad de vehículos por página. Por defecto, 5. Se recorta a un máximo de 50.</param>
        /// <returns>La lista de vehículos activos que cumplen el filtro.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles(
            [FromQuery] int? branchId,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)
        {
            var result = await _vehicleService.GetVehiclesAsync(branchId, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result.Items);
        }

        /// <summary>
        /// Obtiene los detalles de un vehículo por su identificador (activo o inactivo).
        /// </summary>
        /// <param name="id">El identificador único del vehículo.</param>
        /// <returns>Los detalles del vehículo encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetVehicleById(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound(new { message = $"No se encontró un vehículo con el id '{id}'." });
            }

            return Ok(vehicle);
        }

        /// <summary>
        /// Reemplaza por completo los datos editables de un vehículo existente, incluida la
        /// reasignación de sucursal. Puede usarse también para reactivar un vehículo inactivo
        /// enviando <c>isActive: true</c>.
        /// </summary>
        /// <param name="id">El identificador único del vehículo.</param>
        /// <param name="dto">Datos completos de reemplazo del vehículo.</param>
        /// <returns>El vehículo actualizado.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<VehicleDto>> UpdateVehicle(int id, [FromBody] UpdateVehicleDto dto)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicleAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No se encontró un vehículo con el id '{id}'." });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Desactiva un vehículo (soft-delete: <c>isActive = false</c>). Este es el único
        /// mecanismo para retirar un vehículo — nunca se elimina el registro. Idempotente:
        /// repetir la operación sobre un vehículo ya inactivo no produce error.
        /// </summary>
        /// <param name="id">El identificador único del vehículo.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateVehicle(int id)
        {
            var success = await _vehicleService.DeactivateVehicleAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"No se encontró un vehículo con el id '{id}'." });
            }

            return NoContent();
        }
    }
}
