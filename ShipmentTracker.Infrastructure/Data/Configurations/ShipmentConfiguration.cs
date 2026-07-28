using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Infrastructure.Data.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.TrackingNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(x => x.TrackingNumber)
                   .IsUnique();

            builder.Property(x => x.Recipient)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.DeliveredAt)
                   .IsRequired(false);
        }
    }
}
