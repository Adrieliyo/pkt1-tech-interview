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
    public class BranchScheduleConfiguration : IEntityTypeConfiguration<BranchSchedule>
    {
        public void Configure(EntityTypeBuilder<BranchSchedule> builder)
        {
            builder.ToTable("BranchSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.DayOfWeek)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.OpensAt)
                   .IsRequired(false);

            builder.Property(x => x.ClosesAt)
                   .IsRequired(false);

            builder.Property(x => x.IsClosed)
                   .IsRequired();

            builder.HasOne(x => x.Branch)
                   .WithMany(b => b.Schedule)
                   .HasForeignKey(x => x.BranchId)
                   .IsRequired();

            // Defensa en profundidad de FR-005 (sin días duplicados) a nivel de base de datos
            builder.HasIndex(x => new { x.BranchId, x.DayOfWeek })
                   .IsUnique();
        }
    }
}
