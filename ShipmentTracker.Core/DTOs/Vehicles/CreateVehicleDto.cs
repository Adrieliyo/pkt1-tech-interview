using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs.Vehicles
{
    /// <summary>
    /// Datos requeridos para la creación de un nuevo vehículo.
    /// </summary>
    public class CreateVehicleDto
    {
        /// <summary>
        /// Identificador de la sucursal a la que se asigna el vehículo. Debe ser una sucursal existente y activa.
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// Placa del vehículo, única a nivel compañía.
        /// </summary>
        public string Plate { get; set; } = null!;
        /// <summary>
        /// Tipo de vehículo. Nullable para poder distinguir "omitido" de un valor explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VehicleType? Type { get; set; }
        /// <summary>
        /// Marca del vehículo.
        /// </summary>
        public string Brand { get; set; } = null!;
        /// <summary>
        /// Modelo del vehículo.
        /// </summary>
        public string Model { get; set; } = null!;
        /// <summary>
        /// Año de fabricación. No puede ser un año futuro.
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Capacidad máxima de carga, en kilogramos. Debe ser positiva.
        /// </summary>
        public decimal MaxWeightKg { get; set; }
    }
}
