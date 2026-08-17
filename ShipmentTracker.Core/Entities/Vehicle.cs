using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        public int BranchId { get; set; }

        public string Plate { get; set; } = null!;

        public VehicleType Type { get; set; }

        public string Brand { get; set; } = null!;

        public string Model { get; set; } = null!;

        public int Year { get; set; }

        public decimal MaxWeightKg { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public Branch Branch { get; set; } = null!;
    }
}
