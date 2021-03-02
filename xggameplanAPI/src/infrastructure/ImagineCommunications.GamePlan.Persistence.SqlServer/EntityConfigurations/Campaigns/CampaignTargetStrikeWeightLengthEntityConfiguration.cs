using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignTargetStrikeWeightLengthEntityConfiguration : IEntityTypeConfiguration<CampaignTargetStrikeWeightLength>
    {
        public void Configure(EntityTypeBuilder<CampaignTargetStrikeWeightLength> builder)
        {
            builder.ToTable("CampaignTargetStrikeWeightLengths");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Length).AsTicks();
            builder.Property(e => e.DesiredPercentageSplit).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.CurrentPercentageSplit).HasColumnType("DECIMAL(28,18)");

            builder.HasIndex(e => e.CampaignTargetStrikeWeightId);
        }
    }
}
