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
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            // Sin filtro por IsActive: la unicidad aplica siempre, incluso contra registros
            // inactivos (spec.md, Clarifications).
            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.Property(x => x.Phone)
                   .IsRequired(false)
                   .HasMaxLength(30);

            builder.Property(x => x.Role)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.EmployeeNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(x => x.EmployeeNumber)
                   .IsUnique();

            builder.Property(x => x.HireDate)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.UpdatedAt)
                   .IsRequired(false);

            builder.HasOne(x => x.Branch)
                   .WithMany()
                   .HasForeignKey(x => x.BranchId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
