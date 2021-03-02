using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioCampaignPriorityRoundCollectionEntityConfiguration : IEntityTypeConfiguration<ScenarioCampaignPriorityRoundCollection>
    {
        public void Configure(EntityTypeBuilder<ScenarioCampaignPriorityRoundCollection> builder)
        {
            builder.ToTable("ScenarioCampaignPriorityRoundCollections");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.HasMany(e => e.Rounds).WithOne()
                .HasForeignKey(e => e.ScenarioCampaignPriorityRoundCollectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
