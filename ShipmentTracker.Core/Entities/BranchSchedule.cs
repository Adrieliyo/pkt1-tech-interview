using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Entities
{
    public class BranchSchedule
    {
        public int Id { get; set; }

        // FK requerida hacia la sucursal dueña de esta entrada
        public int BranchId { get; set; }

        public ScheduleDay DayOfWeek { get; set; }

        // Hora local de apertura (sin zona horaria); null si IsClosed
        public TimeOnly? OpensAt { get; set; }

        // Hora local de cierre (sin zona horaria); null si IsClosed
        public TimeOnly? ClosesAt { get; set; }

        public bool IsClosed { get; set; }

        public Branch Branch { get; set; } = null!;
    }
}
