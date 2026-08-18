using System;

namespace ShipmentTracker.Core.Entities
{
    public class IndividualCustomer : Customer
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateOnly? BirthDate { get; set; }

        // CURP u otro identificador gubernamental equivalente
        public string GovernmentId { get; set; } = null!;
    }
}
