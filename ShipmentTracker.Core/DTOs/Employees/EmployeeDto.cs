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
    /// Modelo de datos para representar un empleado en la capa de transferencia de datos (DTO).
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Identificador único del empleado.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador de la sucursal a la que pertenece el empleado.
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
        /// Rol del empleado (Operator, Driver, WarehouseStaff o BranchManager).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmployeeRole Role { get; set; }
        /// <summary>
        /// Número de empleado, único a nivel compañía.
        /// </summary>
        public string EmployeeNumber { get; set; } = null!;
        /// <summary>
        /// Fecha de contratación.
        /// </summary>
        public DateOnly HireDate { get; set; }
        /// <summary>
        /// Indica si el empleado está activo. Los empleados nunca se eliminan, solo se desactivan.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Fecha de creación del registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Fecha de la última actualización. Null si nunca se ha actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Indica si el empleado ya tiene una cuenta de acceso (ApplicationUser) vinculada.
        /// Calculado en el servicio, no persistido en la entidad Employee.
        /// </summary>
        public bool HasAccount { get; set; }
    }
}
