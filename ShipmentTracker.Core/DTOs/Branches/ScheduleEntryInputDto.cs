using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs.Branches
{
    /// <summary>
    /// Datos de una entrada del horario semanal para la creación o el reemplazo completo del horario de una sucursal.
    /// </summary>
    public class ScheduleEntryInputDto
    {
        /// <summary>
        /// Día de la semana al que corresponde esta entrada. Nullable para poder distinguir "omitido" de "Monday" explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ScheduleDay? DayOfWeek { get; set; }
        /// <summary>
        /// Indica si la sucursal permanece cerrada ese día. Si es <c>true</c>, <see cref="OpensAt"/> y <see cref="ClosesAt"/> deben omitirse.
        /// </summary>
        public bool IsClosed { get; set; }
        /// <summary>
        /// Hora local de apertura (sin zona horaria). Requerida si <see cref="IsClosed"/> es <c>false</c>.
        /// </summary>
        public TimeOnly? OpensAt { get; set; }
        /// <summary>
        /// Hora local de cierre (sin zona horaria). Requerida si <see cref="IsClosed"/> es <c>false</c>.
        /// </summary>
        public TimeOnly? ClosesAt { get; set; }
    }
}
