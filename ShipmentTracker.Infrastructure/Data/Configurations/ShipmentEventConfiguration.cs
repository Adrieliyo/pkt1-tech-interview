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
    public class ShipmentEventConfiguration : IEntityTypeConfiguration<ShipmentEvent>
    {
        public void Configure(EntityTypeBuilder<ShipmentEvent> builder)
        {
            builder.ToTable("ShipmentEvents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.EventType)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.StatusSnapshot)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.OccurredAt)
                   .IsRequired();

            builder.HasOne(x => x.Shipment)
                   .WithMany()
                   .HasForeignKey(x => x.ShipmentId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.EmployeeId)
                   .IsRequired(false);

            builder.Property(x => x.LocationLabel)
                   .IsRequired(false)
                   .HasMaxLength(255);

            builder.Property(x => x.Notes)
                   .IsRequired(false)
                   .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.Employee)
                   .WithMany()
                   .HasForeignKey(x => x.EmployeeId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}