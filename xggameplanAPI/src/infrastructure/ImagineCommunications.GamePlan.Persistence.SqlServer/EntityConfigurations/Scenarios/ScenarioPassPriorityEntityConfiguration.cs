using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioPassPriorityEntityConfiguration : IEntityTypeConfiguration<ScenarioPassPriority>
    {
        public void Configure(EntityTypeBuilder<ScenarioPassPriority> builder)
        {
            builder.ToTable("ScenarioPassPriorities");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.PassName).HasMaxLength(256);

            builder.HasIndex(e => e.ScenarioCampaignPassPriorityId);
        }
    }
}
