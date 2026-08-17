using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Entities
{
    public class Branch
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public BranchType Type { get; set; }

        // Línea de calle de la dirección
        public string Address { get; set; } = null!;

        public string City { get; set; } = null!;

        // Texto libre, no restringido a una lista fija de códigos
        public string State { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // Horario semanal completo: exactamente 7 entradas, una por día
        public ICollection<BranchSchedule> Schedule { get; set; } = new List<BranchSchedule>();
    }
}
