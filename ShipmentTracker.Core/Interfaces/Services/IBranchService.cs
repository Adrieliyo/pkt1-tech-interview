using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Branches;
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
        /// Lista sucursales de forma paginada, opcionalmente filtradas por tipo. Por defecto solo
        /// devuelve sucursales activas, ordenadas por fecha de creación descendente.
        /// </summary>
        /// <param name="onlyActive">Si es <c>true</c> (por defecto), devuelve solo sucursales activas; si es <c>false</c>, devuelve solo inactivas.</param>
        /// <param name="type">Tipo de sucursal opcional para filtrar los resultados.</param>
        /// <param name="page">Número de página solicitado (1-based).</param>
        /// <param name="pageSize">Tamaño de página solicitado. Se recorta a un máximo de 50.</param>
        Task<PagedResult<BranchDto>> GetBranchesAsync(bool onlyActive = true, BranchType? type = null, int page = 1, int pageSize = 5);

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
