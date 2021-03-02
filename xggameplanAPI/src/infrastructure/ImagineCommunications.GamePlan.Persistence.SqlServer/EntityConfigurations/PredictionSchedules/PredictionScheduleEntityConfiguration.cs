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
            builder.ToTable("PredictionSchedules");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.ScheduleDay).AsUtc();
            builder.Property(e => e.SalesArea).HasMaxLength(64).IsRequired();

            builder.HasMany(e => e.Ratings).WithOne(e => e.PredictionSchedule)
                .HasForeignKey(e => e.PredictionScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SalesArea);
            builder.HasIndex(nameof(PredictionSchedule.SalesArea), nameof(PredictionSchedule.ScheduleDay));
        }
    }
}
