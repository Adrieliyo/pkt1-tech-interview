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

            // Referencia opcional a la orden de origen: null para Shipments creados directamente
            // por POST /api/shipment; asignado solo cuando el Shipment nace de la conversión de una orden.
            builder.Property(x => x.OrderId)
                   .IsRequired(false);

            // Índice único que permite múltiples NULL (comportamiento por defecto de SQL Server),
            // exigido por los 5 Shipments sembrados y la creación directa aún activa.
            builder.HasIndex(x => x.OrderId)
                   .IsUnique();

            builder.HasOne<Order>()
                   .WithMany()
                   .HasForeignKey(x => x.OrderId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
