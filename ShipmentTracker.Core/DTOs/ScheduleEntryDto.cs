using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs
{
    /// <summary>
    /// Representa una entrada del horario semanal de una sucursal en la salida de la API.
    /// </summary>
    public class ScheduleEntryDto
    {
        /// <summary>
        /// Identificador único de la entrada de horario.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Día de la semana al que corresponde esta entrada.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ScheduleDay DayOfWeek { get; set; }
        /// <summary>
        /// Indica si la sucursal permanece cerrada ese día. Si es <c>true</c>, <see cref="OpensAt"/> y <see cref="ClosesAt"/> son <c>null</c>.
        /// </summary>
        public bool IsClosed { get; set; }
        /// <summary>
        /// Hora local de apertura (sin zona horaria). Presente solo si <see cref="IsClosed"/> es <c>false</c>.
        /// </summary>
        public TimeOnly? OpensAt { get; set; }
        /// <summary>
        /// Hora local de cierre (sin zona horaria). Presente solo si <see cref="IsClosed"/> es <c>false</c>.
        /// </summary>
        public TimeOnly? ClosesAt { get; set; }
    }
}
