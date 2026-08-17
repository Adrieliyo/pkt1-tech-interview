using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public int BranchId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public EmployeeRole Role { get; set; }

        public string EmployeeNumber { get; set; } = null!;

        public DateOnly HireDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // Null hasta la primera actualización vía PUT
        public DateTime? UpdatedAt { get; set; }

        public Branch Branch { get; set; } = null!;
    }
}
