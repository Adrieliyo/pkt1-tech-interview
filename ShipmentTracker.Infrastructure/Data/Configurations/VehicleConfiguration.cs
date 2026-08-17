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
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.Plate)
                   .IsRequired()
                   .HasMaxLength(20);

            // Sin filtro por IsActive: la unicidad aplica siempre, incluso contra registros
            // inactivos (spec.md, Clarifications).
            builder.HasIndex(x => x.Plate)
                   .IsUnique();

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.Brand)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Model)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Year)
                   .IsRequired();

            builder.Property(x => x.MaxWeightKg)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.Branch)
                   .WithMany()
                   .HasForeignKey(x => x.BranchId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
