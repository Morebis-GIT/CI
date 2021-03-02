using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ScenarioResults
{
    public class ScenarioCampaignResultEntityConfiguration : IEntityTypeConfiguration<ScenarioCampaignResult>
    {
        public void Configure(EntityTypeBuilder<ScenarioCampaignResult> builder)
        {
            _ = builder.ToTable("ScenarioCampaignResults");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            _ = builder.Property(e => e.CampaignExternalId).HasMaxLength(64);
            _ = builder.Property(e => e.SalesAreaName).HasMaxLength(128);
            _ = builder.Property(e => e.DaypartName).HasMaxLength(512);

            _ = builder.Property(e => e.StrikeWeightEndDate).AsUtc();
            _ = builder.Property(e => e.StrikeWeightStartDate).AsUtc();

            _ = builder.HasIndex(e => e.ScenarioId);
        }
    }
}
