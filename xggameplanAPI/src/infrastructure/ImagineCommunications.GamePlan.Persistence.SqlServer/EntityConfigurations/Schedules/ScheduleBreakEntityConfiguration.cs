using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Schedules
{
    public class ScheduleBreakEntityConfiguration : IEntityTypeConfiguration<ScheduleBreak>
    {
        public void Configure(EntityTypeBuilder<ScheduleBreak> builder)
        {
            _ = builder.ToTable("ScheduleBreaks");

            _ = builder.HasKey(e => e.Id);

            _ = builder.Property(e => e.BreakType)
                       .HasMaxLength(32);

            _ = builder.Property(e => e.Duration)
                       .AsTicks();

            _ = builder.Property(c => c.ReserveDuration)
                       .AsTicks();

            _ = builder.Property(e => e.Avail)
                       .AsTicks();

            _ = builder.Property(e => e.OptimizerAvail)
                       .AsTicks();

            _ = builder.Property(e => e.ExternalBreakRef)
                       .HasMaxLength(64);

            _ = builder.Property(e => e.Description)
                       .HasMaxLength(512);

            _ = builder.Property(e => e.ExternalProgRef)
                       .HasMaxLength(64);

            _ = builder.Property(e => e.BreakPrice)
                       .HasDefaultValue();

            _ = builder.Property(e => e.FloorRate)
                       .HasDefaultValue();

            _ = builder.Property(e => e.ScheduledDate)
                       .AsUtc();

            _ = builder.HasIndex(e => e.ExternalBreakRef)
                       .IsUnique();

            _ = builder.HasIndex(e => e.ExternalProgRef);
            _ = builder.HasIndex(e => e.SalesAreaId);
            _ = builder.HasIndex(e => e.ScheduledDate);
            _ = builder.HasIndex(e => e.ScheduleId);

            _ = builder.HasMany(x => x.BreakEfficiencies)
                       .WithOne()
                       .HasForeignKey(x => x.ScheduleBreakId)
                       .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
