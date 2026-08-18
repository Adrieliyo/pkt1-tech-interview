using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Entities
{
    public class Shipment
    {
        public int Id { get; set; }

        // Numero de guia
        public string TrackingNumber { get; set; } = null!;

        // Destinatario
        public string Recipient { get; set; } = null!;

        // Estado del envio
        public ShipmentStatus Status { get; set; }

        // Fecha de creacion del envio
        public DateTime CreatedAt { get; set; }

        // Fecha de entrega del envio
        public DateTime? DeliveredAt { get; set; }

        // Orden de origen (nullable: los Shipments creados directamente por POST /api/shipment no tienen orden)
        public int? OrderId { get; set; }
    }
}
