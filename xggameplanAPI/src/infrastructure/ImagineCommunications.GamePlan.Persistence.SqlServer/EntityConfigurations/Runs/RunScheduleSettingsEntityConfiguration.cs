using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunScheduleSettingsEntityConfiguration : IEntityTypeConfiguration<RunScheduleSettings>
    {
        public void Configure(EntityTypeBuilder<RunScheduleSettings> builder)
        {
            builder.ToTable("RunScheduleSettings");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.QueueName).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Comment).HasMaxLength(512);

            builder.HasOne(p => p.Run)
                .WithOne(p => p.ScheduleSettings)
                .HasForeignKey<RunScheduleSettings>(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
