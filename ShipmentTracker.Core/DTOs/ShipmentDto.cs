using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public string TrackingNumber { get; set; } = null!;
        public string Recipient { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
