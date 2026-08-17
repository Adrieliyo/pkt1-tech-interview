using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces.Services
{
    public interface IBranchService
    {
        /// <summary>
        /// Crea una nueva sucursal. El servicio marca la sucursal como activa y asigna CreatedAt.
        /// </summary>
        Task<BranchDto> CreateBranchAsync(CreateBranchDto dto);

        /// <summary>
        /// Lista sucursales, opcionalmente filtradas por tipo. Por defecto solo devuelve sucursales activas.
        /// </summary>
        Task<IEnumerable<BranchDto>> GetBranchesAsync(bool onlyActive = true, BranchType? type = null);

        /// <summary>
        /// Obtiene una sucursal por su identificador, incluido su horario completo.
        /// </summary>
        Task<BranchDto?> GetBranchByIdAsync(int id);

        /// <summary>
        /// Reemplaza por completo los datos editables de una sucursal existente, incluido su
        /// horario semanal. No restringe la edición según el estado activo/inactivo actual.
        /// </summary>
        Task<BranchDto?> UpdateBranchAsync(int id, UpdateBranchDto dto);

        /// <summary>
        /// Desactiva una sucursal (soft-delete). Nunca elimina el registro. Idempotente: repetir
        /// la operación sobre una sucursal ya inactiva no produce error.
        /// </summary>
        Task<bool> DeactivateBranchAsync(int id);
    }
}
