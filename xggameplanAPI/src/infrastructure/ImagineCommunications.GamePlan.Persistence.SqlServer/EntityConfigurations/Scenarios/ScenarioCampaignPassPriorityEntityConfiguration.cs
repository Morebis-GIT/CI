using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioCampaignPassPriorityEntityConfiguration : IEntityTypeConfiguration<ScenarioCampaignPassPriority>
    {
        public void Configure(EntityTypeBuilder<ScenarioCampaignPassPriority> builder)
        {
            builder.ToTable("ScenarioCampaignPassPriorities");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasOne(e => e.Campaign)
                .WithOne()
                .HasForeignKey<ScenarioCompactCampaign>(e => e.ScenarioCampaignPassPriorityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PassPriorities)
                .WithOne()
                .HasForeignKey(e => e.ScenarioCampaignPassPriorityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.ScenarioId);
        }
    }
}
