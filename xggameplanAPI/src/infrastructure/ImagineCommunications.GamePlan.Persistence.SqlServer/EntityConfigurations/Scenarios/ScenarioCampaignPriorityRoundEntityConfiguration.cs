using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioCampaignPriorityRoundEntityConfiguration : IEntityTypeConfiguration<ScenarioCampaignPriorityRound>
    {
        public void Configure(EntityTypeBuilder<ScenarioCampaignPriorityRound> builder)
        {
            builder.ToTable("ScenarioCampaignPriorityRounds");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.ScenarioCampaignPriorityRoundCollectionId);
        }
    }
}
