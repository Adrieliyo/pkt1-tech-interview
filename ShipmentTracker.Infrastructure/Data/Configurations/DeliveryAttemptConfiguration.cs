using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Infrastructure.Data.Configurations
{
    public class DeliveryAttemptConfiguration : IEntityTypeConfiguration<DeliveryAttempt>
    {
        public void Configure(EntityTypeBuilder<DeliveryAttempt> builder)
        {
            builder.ToTable("DeliveryAttempts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn(); // Autoincrementable

            builder.Property(x => x.AttemptNumber)
                   .IsRequired();

            builder.Property(x => x.FailureReason)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.NextAttemptAt)
                   .IsRequired(false);

            // Relación uno a uno: un DeliveryAttempt por ShipmentEvent.
            builder.HasOne(x => x.ShipmentEvent)
                   .WithMany()
                   .HasForeignKey(x => x.ShipmentEventId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ShipmentEventId)
                   .IsUnique();
        }
    }
}
