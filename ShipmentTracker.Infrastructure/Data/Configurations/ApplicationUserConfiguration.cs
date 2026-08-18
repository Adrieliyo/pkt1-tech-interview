using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Core.Identity;

namespace ShipmentTracker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración adicional de ApplicationUser sobre la ya provista por IdentityDbContext:
    /// la relación con Employee (nullable — ausente para SuperAdmin, research.md Decisión 2) y su
    /// unicidad (un ApplicationUser por Employee, research.md Decisión 2 / data-model.md).
    /// </summary>
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(x => x.Employee)
                   .WithMany()
                   .HasForeignKey(x => x.EmployeeId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.EmployeeId)
                   .IsUnique()
                   .HasFilter("[EmployeeId] IS NOT NULL");
        }
    }
}
