using Microsoft.EntityFrameworkCore;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ScenarioResults
{
    public class ScenarioCampaignMetricEntityConfiguration : IEntityTypeConfiguration<ScenarioCampaignMetric>
    {
        public void Configure(EntityTypeBuilder<ScenarioCampaignMetric> builder)
        {
            builder.ToTable("ScenarioCampaignMetrics");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.CampaignExternalId).HasMaxLength(64);

            builder.HasIndex(e => e.ScenarioId);
        }
    }
}
