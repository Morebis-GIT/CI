using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Breaks
{
    public class BreakEntityConfiguration : IEntityTypeConfiguration<Break>
    {
        public void Configure(EntityTypeBuilder<Break> builder)
        {
            _ = builder.ToTable("Breaks")
                       .HasKey(e => e.Id);

            _ = builder.Property(e => e.Id)
                       .HasDefaultValueSql("newid()");

            _ = builder.Property(e => e.BreakType)
                       .HasMaxLength(32);

            _ = builder.Property(e => e.Duration)
                       .AsTicks();

            _ = builder.Property(e => e.Avail)
                       .AsTicks();

            _ = builder.Property(e => e.OptimizerAvail)
                       .AsTicks();

            _ = builder.Property(e => e.ScheduledDate)
                       .AsUtc();

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

            _ = builder.HasMany(x => x.BreakEfficiencies)
                       .WithOne()
                       .HasForeignKey(x => x.BreakId)
                       .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(e => e.ExternalBreakRef)
                       .IsUnique();

            _ = builder.HasIndex(e => e.ExternalProgRef);
            _ = builder.HasIndex(e => e.SalesAreaId);
            _ = builder.HasIndex(e => e.ScheduledDate);
            _ = builder.HasIndex(
                    nameof(Break.SalesAreaId),
                    nameof(Break.ScheduledDate),
                    nameof(Break.CustomId),
                    nameof(Break.BreakType),
                    nameof(Break.Duration),
                    nameof(Break.Avail),
                    nameof(Break.OptimizerAvail),
                    nameof(Break.Optimize),
                    nameof(Break.ExternalBreakRef),
                    nameof(Break.Description),
                    nameof(Break.ExternalProgRef),
                    nameof(Break.PositionInProg)
                ).HasName("IX_ScheduleBreaks_Summary");
        }
    }
}
