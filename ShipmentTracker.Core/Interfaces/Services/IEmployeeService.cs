using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces.Services
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Crea un nuevo empleado. El servicio marca el empleado como activo y asigna CreatedAt.
        /// </summary>
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);

        /// <summary>
        /// Lista empleados de forma paginada, opcionalmente filtrados por sucursal, por rol, o
        /// por ambos. Solo devuelve empleados activos.
        /// </summary>
        Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int? branchId = null, EmployeeRole? role = null, int page = 1, int pageSize = 5);

        /// <summary>
        /// Obtiene un empleado por su identificador, activo o inactivo.
        /// </summary>
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

        /// <summary>
        /// Reemplaza por completo los datos editables de un empleado existente, incluida la
        /// reasignación de sucursal. No restringe la edición según el estado activo/inactivo actual.
        /// </summary>
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);

        /// <summary>
        /// Desactiva un empleado (soft-delete). Nunca elimina el registro. Idempotente: repetir
        /// la operación sobre un empleado ya inactivo no produce error.
        /// </summary>
        Task<bool> DeactivateEmployeeAsync(int id);
    }
}
