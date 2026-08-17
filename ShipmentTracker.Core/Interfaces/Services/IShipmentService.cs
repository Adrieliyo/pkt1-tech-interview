using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Shipments;
using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces.Services
{
    public interface IShipmentService
    {
        /// <summary>
        /// Lista los envíos de forma paginada. Permite filtrar opcionalmente por el estado del envío
        /// usando el Enum. Los envíos se devuelven ordenados por fecha de creación descendente.
        /// </summary>
        /// <param name="status">Estado opcional para filtrar los envíos.</param>
        /// <param name="page">Número de página solicitado (1-based).</param>
        /// <param name="pageSize">Tamaño de página solicitado.</param>
        Task<PagedResult<ShipmentDto>> GetShipmentsAsync(ShipmentStatus? status = null, int page = 1, int pageSize = 5);

        /// <summary>
        /// Obtiene un envío por su número de guía.
        /// </summary>
        Task<ShipmentDto?> GetShipmentByTrackingNumberAsync(string trackingNumber);

        /// <summary>
        /// Crea un nuevo envío. El servicio se encargará de generar el TrackingNumber y asignar CreatedAt.
        /// </summary>
        Task<ShipmentDto> CreateShipmentAsync(CreateShipmentDto createShipmentDto);

        /// <summary>
        /// Actualiza el estado de un envío.
        /// </summary>
        Task<bool> UpdateShipmentStatusAsync(string trackingNumber, ShipmentStatus newStatus);
    }
}
