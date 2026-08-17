using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs.Employees
{
    /// <summary>
    /// Datos para el reemplazo completo de un empleado existente (PUT).
    /// </summary>
    public class UpdateEmployeeDto
    {
        /// <summary>
        /// Identificador de la sucursal a la que pertenece el empleado. Debe ser una sucursal existente y activa.
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// Nombre(s) del empleado.
        /// </summary>
        public string FirstName { get; set; } = null!;
        /// <summary>
        /// Apellido(s) del empleado.
        /// </summary>
        public string LastName { get; set; } = null!;
        /// <summary>
        /// Correo electrónico del empleado, único a nivel compañía.
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Teléfono de contacto, opcional.
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Rol del empleado. Puede cambiarse a cualquiera de los 4 valores en cualquier momento.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmployeeRole? Role { get; set; }
        /// <summary>
        /// Número de empleado, único a nivel compañía.
        /// </summary>
        public string EmployeeNumber { get; set; } = null!;
        /// <summary>
        /// Fecha de contratación.
        /// </summary>
        public DateOnly HireDate { get; set; }
        /// <summary>
        /// Estado activo/inactivo. Permite reactivar un empleado desactivado.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
