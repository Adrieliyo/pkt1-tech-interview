using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Core.Entities
{
    public abstract class Customer
    {
        public int Id { get; set; }

        public CustomerType Type { get; set; }

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        // Línea de calle de la dirección
        public string Address { get; set; } = null!;

        public string City { get; set; } = null!;

        // Texto libre, no restringido a una lista fija de códigos
        public string State { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        public string Country { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // Null hasta la primera actualización vía PUT
        public DateTime? UpdatedAt { get; set; }
    }
}
