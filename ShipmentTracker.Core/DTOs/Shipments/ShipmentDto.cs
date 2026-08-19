using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs.Shipments
{
    /// <summary>
    /// Modelo de datos para representar un envío en la capa de transferencia de datos (DTO).
    /// </summary>
    public class ShipmentDto
    {
        /// <summary>
        /// Representa el identificador único del envío.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Representa el numero de guía o tracking del envío.
        /// </summary>
        public string TrackingNumber { get; set; } = null!;
        /// <summary>
        /// Representa el nombre completo de la persona o empresa que recibirá el envío.
        /// </summary>
        public string Recipient { get; set; } = null!;
        /// <summary>
        /// Representa el estado actual del envío, utilizando el Enum ShipmentStatus.
        /// Siendo los posibles valores: Collected, InTransit, Delivered y Cancelled.
        /// </summary>
        public ShipmentStatus Status { get; set; }
        /// <summary>
        /// Representa la hora en la que el envío fue creado. Este valor es asignado automáticamente al momento de la creación del envío.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Representa la hora en la que el envío fue entregado. Este valor es opcional y solo se asigna cuando el estado del envío cambia a "Delivered".
        /// </summary>
        public DateTime? DeliveredAt { get; set; }
        /// <summary>
        /// Identificador de la Order que generó este envío mediante ConvertToShipmentAsync. Null para
        /// los envíos creados directamente vía POST /api/shipment (sin pasar por una orden).
        /// </summary>
        public int? OrderId { get; set; }
    }
}
