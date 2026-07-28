using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs
{
    public class CreateShipmentDto
    {
        public string Recipient { get; set; } = null!;
    }
}
