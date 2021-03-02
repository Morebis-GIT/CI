using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignBreakRequirementEntityConfiguration : IEntityTypeConfiguration<CampaignBreakRequirement>
    {
        public void Configure(EntityTypeBuilder<CampaignBreakRequirement> builder)
        {
            builder.ToTable("CampaignBreakRequirements");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.SalesArea).HasMaxLength(64);
        }
    }
}
