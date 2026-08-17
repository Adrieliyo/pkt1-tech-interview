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
    /// Modelo de datos para representar un vehículo en la capa de transferencia de datos (DTO).
    /// </summary>
    public class VehicleDto
    {
        /// <summary>
        /// Identificador único del vehículo.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador de la sucursal a la que está asignado el vehículo.
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// Placa del vehículo, única a nivel compañía.
        /// </summary>
        public string Plate { get; set; } = null!;
        /// <summary>
        /// Tipo de vehículo (Motorcycle, Van o Truck).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VehicleType Type { get; set; }
        /// <summary>
        /// Marca del vehículo.
        /// </summary>
        public string Brand { get; set; } = null!;
        /// <summary>
        /// Modelo del vehículo.
        /// </summary>
        public string Model { get; set; } = null!;
        /// <summary>
        /// Año de fabricación.
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Capacidad máxima de carga, en kilogramos.
        /// </summary>
        public decimal MaxWeightKg { get; set; }
        /// <summary>
        /// Indica si el vehículo está activo. Los vehículos nunca se eliminan, solo se desactivan.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Fecha de creación del registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
