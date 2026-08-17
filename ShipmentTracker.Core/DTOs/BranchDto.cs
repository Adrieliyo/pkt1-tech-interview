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
    /// Modelo de datos para representar una sucursal en la capa de transferencia de datos (DTO).
    /// </summary>
    public class BranchDto
    {
        /// <summary>
        /// Identificador único de la sucursal.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre de la sucursal.
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Tipo de sucursal (Headquarters, Hub, SalesPoint o PickupPoint).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BranchType Type { get; set; }
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
        /// Indica si la sucursal está activa. Las sucursales nunca se eliminan, solo se desactivan.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Fecha de creación de la sucursal.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Horario semanal completo de la sucursal (siempre 7 entradas, una por día).
        /// </summary>
        public List<ScheduleEntryDto> Schedule { get; set; } = new List<ScheduleEntryDto>();
    }
}
