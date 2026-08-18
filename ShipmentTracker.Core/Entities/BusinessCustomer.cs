namespace ShipmentTracker.Core.Entities
{
    public class BusinessCustomer : Customer
    {
        public string BusinessName { get; set; } = null!;

        // RFC (persona moral)
        public string TaxId { get; set; } = null!;

        public string LegalRepresentative { get; set; } = null!;

        public string? Industry { get; set; }

        public decimal? CreditLimit { get; set; }
    }
}
