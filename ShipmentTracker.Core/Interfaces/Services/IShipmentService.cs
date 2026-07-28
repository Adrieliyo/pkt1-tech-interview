using ShipmentTracker.Core.DTOs;
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
        /// Lista los envíos. Permite filtrar opcionalmente por el estado del envío usando el Enum.
        /// </summary>
        Task<IEnumerable<ShipmentDto>> GetShipmentsAsync(ShipmentStatus? status = null);

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
