using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignBreakRequirementItemEntityConfiguration : IEntityTypeConfiguration<CampaignBreakRequirementItem>
    {
        public void Configure(EntityTypeBuilder<CampaignBreakRequirementItem> builder)
        {
            builder.ToTable("CampaignBreakRequirementItems")
                .HasDiscriminator<int>("Discriminator")
                .HasValue<CampaignCentreBreakRequirementItem>(1)
                .HasValue<CampaignEndBreakRequirementItem>(2);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
        }
    }
}
