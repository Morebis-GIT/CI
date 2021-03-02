using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioCompactCampaignPaybackEntityConfiguration : IEntityTypeConfiguration<ScenarioCompactCampaignPayback>
    {
        public void Configure(EntityTypeBuilder<ScenarioCompactCampaignPayback> builder)
        {
            builder.ToTable("ScenarioCompactCampaignPaybacks");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.ScenarioCompactCampaignId);
        }
    }
}
