using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Infrastructure.Data.Configurations
{
    public class BusinessCustomerConfiguration : IEntityTypeConfiguration<BusinessCustomer>
    {
        public void Configure(EntityTypeBuilder<BusinessCustomer> builder)
        {
            builder.ToTable("BusinessCustomers");

            builder.Property(x => x.BusinessName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.TaxId)
                   .IsRequired()
                   .HasMaxLength(12); // RFC persona moral

            builder.HasIndex(x => x.TaxId)
                   .IsUnique();

            builder.Property(x => x.LegalRepresentative)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Industry)
                   .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(x => x.CreditLimit)
                   .IsRequired(false)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
