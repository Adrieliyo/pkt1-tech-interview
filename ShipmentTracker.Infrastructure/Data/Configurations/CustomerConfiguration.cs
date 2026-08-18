using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            // Sin filtro por IsActive: la unicidad aplica siempre, incluso contra registros
            // inactivos (spec.md, Clarifications).
            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.Property(x => x.Phone)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(x => x.Address)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.State)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.ZipCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.UpdatedAt)
                   .IsRequired(false);
        }
    }
}
