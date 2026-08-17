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
    /// Datos requeridos para la creación de una nueva sucursal.
    /// </summary>
    public class CreateBranchDto
    {
        /// <summary>
        /// Nombre de la sucursal.
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Tipo de sucursal. Nullable para poder distinguir "omitido" de un valor explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BranchType? Type { get; set; }
        /// <summary>
        /// Línea de calle de la dirección.
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Ciudad de la dirección.
        /// </summary>
        public string City { get; set; } = null!;
        /// <summary>
        /// Estado o provincia de la dirección (texto libre).
        /// </summary>
        public string State { get; set; } = null!;
        /// <summary>
        /// Código postal de la dirección.
        /// </summary>
        public string ZipCode { get; set; } = null!;
        /// <summary>
        /// Latitud geográfica, opcional.
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Longitud geográfica, opcional.
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Teléfono de contacto, opcional.
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Horario semanal completo (exactamente 7 entradas, una por día).
        /// </summary>
        public List<ScheduleEntryInputDto> Schedule { get; set; } = new List<ScheduleEntryInputDto>();
    }
}
