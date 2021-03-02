using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunLandmarkScheduleSettingsEntityConfiguration : IEntityTypeConfiguration<RunLandmarkScheduleSettings>
    {
        public void Configure(EntityTypeBuilder<RunLandmarkScheduleSettings> builder)
        {
            builder.ToTable("RunLandmarkScheduleSettings");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.QueueName).HasMaxLength(64);
            builder.Property(x => x.DaysOfWeek).IsRequired().AsStringPattern(DayOfWeek.Sunday);
            builder.Property(x => x.ScheduledTime).AsTicks();
            builder.Property(x => x.Comment).HasMaxLength(512);

            builder.HasOne(p => p.RunType)
                .WithOne(p => p.RunLandmarkScheduleSettings)
                .HasForeignKey<RunLandmarkScheduleSettings>(p => p.RunTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
