using ShipmentTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces.Services
{
    public interface IVehicleService
    {
        /// <summary>
        /// Crea un nuevo vehículo. El servicio marca el vehículo como activo y asigna CreatedAt.
        /// </summary>
        Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);

        /// <summary>
        /// Lista vehículos de forma paginada, opcionalmente filtrados por sucursal. Solo devuelve
        /// vehículos activos.
        /// </summary>
        Task<PagedResult<VehicleDto>> GetVehiclesAsync(int? branchId = null, int page = 1, int pageSize = 5);

        /// <summary>
        /// Obtiene un vehículo por su identificador, activo o inactivo.
        /// </summary>
        Task<VehicleDto?> GetVehicleByIdAsync(int id);

        /// <summary>
        /// Reemplaza por completo los datos editables de un vehículo existente, incluida la
        /// reasignación de sucursal.
        /// </summary>
        Task<VehicleDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto);

        /// <summary>
        /// Desactiva un vehículo (soft-delete). Nunca elimina el registro. Idempotente: repetir
        /// la operación sobre un vehículo ya inactivo no produce error.
        /// </summary>
        Task<bool> DeactivateVehicleAsync(int id);
    }
}
