using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.Employees;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones relacionadas con los empleados.
    /// Gestión de Employees restringida a BranchManager en su totalidad (spec 008 FR-013/FR-014/FR-015).
    /// </summary>
    [Route("api/employees")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = Roles.BranchManager)]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Crea un nuevo empleado, asignado a una sucursal activa.
        /// </summary>
        /// <param name="dto">Datos del empleado a crear.</param>
        /// <returns>El empleado recién creado.</returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            try
            {
                var result = await _employeeService.CreateEmployeeAsync(dto);

                // Se usa Created(uri, body) en vez de CreatedAtAction(nameof(...)) para no
                // depender de la acción GetEmployeeById, que se agrega recién en la Historia 2.
                return Created($"/api/employees/{result.Id}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Lista empleados de forma paginada. Por defecto solo devuelve empleados activos;
        /// <paramref name="onlyActive"/> en <c>false</c> devuelve solo los inactivos (mismo
        /// patrón que <c>GET /api/branches</c>). Permite filtrar opcionalmente por sucursal, por
        /// rol, o por ambos de forma combinada — esta combinación es la usada para buscar
        /// choferes disponibles en una sucursal antes de asignarlos a un envío. La metadata de
        /// paginación se expone en los encabezados <c>X-Total-Count</c>, <c>X-Page</c>,
        /// <c>X-Page-Size</c> y <c>X-Total-Pages</c>.
        /// </summary>
        /// <param name="onlyActive">Si es <c>true</c> (por defecto) devuelve solo activos; si es <c>false</c>, solo inactivos.</param>
        /// <param name="branchId">Identificador de sucursal opcional para filtrar los resultados.</param>
        /// <param name="role">Rol opcional para filtrar los resultados.</param>
        /// <param name="page">Número de página solicitada (1-based). Por defecto, 1.</param>
        /// <param name="pageSize">Cantidad de empleados por página. Por defecto, 5. Se recorta a un máximo de 50.</param>
        /// <returns>La lista de empleados que cumplen los filtros.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(
            [FromQuery] bool onlyActive = true,
            [FromQuery] int? branchId = null,
            [FromQuery] EmployeeRole? role = null,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)
        {
            var result = await _employeeService.GetEmployeesAsync(onlyActive, branchId, role, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result.Items);
        }

        /// <summary>
        /// Obtiene los detalles de un empleado por su identificador (activo o inactivo).
        /// </summary>
        /// <param name="id">El identificador único del empleado.</param>
        /// <returns>Los detalles del empleado encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new { message = $"No se encontró un empleado con el id '{id}'." });
            }

            return Ok(employee);
        }

        /// <summary>
        /// Reemplaza por completo los datos editables de un empleado existente, incluida la
        /// reasignación de sucursal. Puede usarse también para reactivar un empleado inactivo
        /// enviando <c>isActive: true</c>.
        /// </summary>
        /// <param name="id">El identificador único del empleado.</param>
        /// <param name="dto">Datos completos de reemplazo del empleado.</param>
        /// <returns>El empleado actualizado.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            try
            {
                var result = await _employeeService.UpdateEmployeeAsync(id, dto);

                if (result == null)
                {
                    return NotFound(new { message = $"No se encontró un empleado con el id '{id}'." });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
        }

        /// <summary>
        /// Desactiva un empleado (soft-delete: <c>isActive = false</c>). Este es el único
        /// mecanismo para retirar un empleado — nunca se elimina el registro. Idempotente:
        /// repetir la operación sobre un empleado ya inactivo no produce error.
        /// </summary>
        /// <param name="id">El identificador único del empleado.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var success = await _employeeService.DeactivateEmployeeAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"No se encontró un empleado con el id '{id}'." });
            }

            return NoContent();
        }
    }
}
