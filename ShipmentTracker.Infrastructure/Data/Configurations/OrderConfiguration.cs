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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(x => x.OrderNumber)
                   .IsUnique();

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.ServiceType)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.PickupType)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.PickupAddress)
                   .IsRequired(false)
                   .HasMaxLength(500);

            builder.Property(x => x.PickupScheduledAt)
                   .IsRequired(false);

            builder.Property(x => x.RecipientName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.RecipientPhone)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.RecipientAddress)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.RecipientCity)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.RecipientState)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.RecipientZipCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.DeclaredWeightKg)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DeclaredWidthCm)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DeclaredHeightCm)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DeclaredLengthCm)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.QuotedPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Notes)
                   .IsRequired(false)
                   .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.UpdatedAt)
                   .IsRequired(false);

            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(x => x.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Branch>()
                   .WithMany()
                   .HasForeignKey(x => x.OriginBranchId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}