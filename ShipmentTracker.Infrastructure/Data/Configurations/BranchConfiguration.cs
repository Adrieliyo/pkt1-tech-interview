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
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasConversion<string>();

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

            builder.Property(x => x.Latitude)
                   .IsRequired(false);

            builder.Property(x => x.Longitude)
                   .IsRequired(false);

            builder.Property(x => x.Phone)
                   .IsRequired(false)
                   .HasMaxLength(30);

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();
        }
    }
}
