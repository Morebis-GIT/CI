using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.PredictionSchedules
{
    public class PredictionScheduleEntityConfiguration : IEntityTypeConfiguration<PredictionSchedule>
    {
        public void Configure(EntityTypeBuilder<PredictionSchedule> builder)
        {
            _ = builder.ToTable("PredictionSchedules");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.Property(e => e.ScheduleDay).AsUtc();

            _ = builder.HasMany(e => e.Ratings).WithOne(e => e.PredictionSchedule)
                .HasForeignKey(e => e.PredictionScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasIndex(x => x.SalesAreaId);
            _ = builder.HasIndex(nameof(PredictionSchedule.SalesAreaId), nameof(PredictionSchedule.ScheduleDay));
            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
