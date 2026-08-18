using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Infrastructure.Data.Configurations
{
    public class IndividualCustomerConfiguration : IEntityTypeConfiguration<IndividualCustomer>
    {
        public void Configure(EntityTypeBuilder<IndividualCustomer> builder)
        {
            builder.ToTable("IndividualCustomers");

            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.BirthDate)
                   .IsRequired(false);

            builder.Property(x => x.GovernmentId)
                   .IsRequired()
                   .HasMaxLength(18); // Longitud del CURP

            // Sin filtro por IsActive: la unicidad aplica siempre, incluso contra registros
            // inactivos (spec.md, Clarifications).
            builder.HasIndex(x => x.GovernmentId)
                   .IsUnique();
        }
    }
}
