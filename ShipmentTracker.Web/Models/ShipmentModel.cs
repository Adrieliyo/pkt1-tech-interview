using ShipmentTracker.Core.Enums;

namespace ShipmentTracker.Web.Models
{
    public class ShipmentModel
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
    }
}
