using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs.Branches;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones relacionadas con las sucursales (Branches &amp; Hubs).
    /// </summary>
    [Route("api/branches")]
    [ApiController]
    [Produces("application/json")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        /// <summary>
        /// Crea una nueva sucursal, incluido su horario semanal completo (7 días).
        /// </summary>
        /// <param name="dto">Datos de la sucursal a crear.</param>
        /// <returns>La sucursal recién creada.</returns>
        [HttpPost]
        public async Task<ActionResult<BranchDto>> CreateBranch([FromBody] CreateBranchDto dto)
        {
            try
            {
                var result = await _branchService.CreateBranchAsync(dto);

                // Se usa Created(uri, body) en vez de CreatedAtAction(nameof(...)) para no
                // depender de la acción GetBranchById, que se agrega recién en la Historia 2.
                return Created($"/api/branches/{result.Id}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista sucursales de forma paginada. Por defecto solo devuelve sucursales activas;
        /// permite filtrar por estado activo/inactivo y por tipo de sucursal, de forma combinada.
        /// Las sucursales se devuelven ordenadas por fecha de creación descendente (más recientes
        /// primero). La metadata de paginación (total de registros, página actual, tamaño de
        /// página y total de páginas) se expone en los encabezados <c>X-Total-Count</c>,
        /// <c>X-Page</c>, <c>X-Page-Size</c> y <c>X-Total-Pages</c>.
        /// </summary>
        /// <param name="onlyActive">Si es <c>true</c> (por defecto), devuelve solo sucursales activas; si es <c>false</c>, devuelve solo inactivas.</param>
        /// <param name="type">Tipo de sucursal opcional para filtrar los resultados.</param>
        /// <param name="page">Número de página solicitada (1-based). Por defecto, 1.</param>
        /// <param name="pageSize">Cantidad de sucursales por página. Por defecto, 5. Se recorta a un máximo de 50.</param>
        /// <returns>Una lista de sucursales correspondiente a la página solicitada.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches(
            [FromQuery] bool onlyActive = true,
            [FromQuery] BranchType? type = null,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)
        {
            var result = await _branchService.GetBranchesAsync(onlyActive, type, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result.Items);
        }

        /// <summary>
        /// Obtiene los detalles de una sucursal por su identificador, incluido su horario semanal completo.
        /// </summary>
        /// <param name="id">El identificador único de la sucursal.</param>
        /// <returns>Los detalles de la sucursal encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDto>> GetBranchById(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);

            if (branch == null)
            {
                return NotFound(new { message = $"No se encontró una sucursal con el id '{id}'." });
            }

            return Ok(branch);
        }

        /// <summary>
        /// Reemplaza por completo los datos editables de una sucursal existente, incluido su
        /// horario semanal. Puede usarse también para reactivar una sucursal inactiva enviando
        /// <c>isActive: true</c> — no existe una acción "activate" separada.
        /// </summary>
        /// <param name="id">El identificador único de la sucursal.</param>
        /// <param name="dto">Datos completos de reemplazo de la sucursal.</param>
        /// <returns>La sucursal actualizada.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BranchDto>> UpdateBranch(int id, [FromBody] UpdateBranchDto dto)
        {
            try
            {
                var result = await _branchService.UpdateBranchAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No se encontró una sucursal con el id '{id}'." });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Desactiva una sucursal (soft-delete: <c>isActive = false</c>). Este es el único
        /// mecanismo para retirar una sucursal — nunca se elimina el registro ni su horario.
        /// Idempotente: repetir la operación sobre una sucursal ya inactiva no produce error.
        /// </summary>
        /// <param name="id">El identificador único de la sucursal.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateBranch(int id)
        {
            var success = await _branchService.DeactivateBranchAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"No se encontró una sucursal con el id '{id}'." });
            }

            return NoContent();
        }
    }
}
