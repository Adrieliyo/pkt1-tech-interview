using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;

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
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista sucursales. Por defecto solo devuelve sucursales activas; permite filtrar por
        /// estado activo/inactivo y por tipo de sucursal, de forma combinada.
        /// </summary>
        /// <param name="onlyActive">Si es <c>true</c> (por defecto), devuelve solo sucursales activas; si es <c>false</c>, devuelve solo inactivas.</param>
        /// <param name="type">Tipo de sucursal opcional para filtrar los resultados.</param>
        /// <returns>La lista de sucursales que cumplen los filtros.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches([FromQuery] bool onlyActive = true, [FromQuery] BranchType? type = null)
        {
            var result = await _branchService.GetBranchesAsync(onlyActive, type);
            return Ok(result);
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
            catch (ValidationException ex)
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
