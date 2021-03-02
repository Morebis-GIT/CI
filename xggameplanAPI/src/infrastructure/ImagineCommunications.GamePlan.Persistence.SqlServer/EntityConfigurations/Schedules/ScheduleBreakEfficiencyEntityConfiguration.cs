using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Schedules
{
    public class ScheduleBreakEfficiencyEntityConfiguration : IEntityTypeConfiguration<ScheduleBreakEfficiency>
    {
        public void Configure(EntityTypeBuilder<ScheduleBreakEfficiency> builder)
        {
            builder.ToTable("ScheduleBreakEfficiencies");

            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(x => x.Demographic).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.ScheduleBreakId);
            builder.HasIndex(e => e.Demographic);
            builder.HasIndex(e => new { e.ScheduleBreakId, e.Demographic, e.Efficiency });
        }
    }
}
