using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignTargetStrikeWeightEntityConfiguration : IEntityTypeConfiguration<CampaignTargetStrikeWeight>
    {
        public void Configure(EntityTypeBuilder<CampaignTargetStrikeWeight> builder)
        {
            builder.ToTable("CampaignTargetStrikeWeights");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.StartDate).AsUtc();
            builder.Property(e => e.EndDate).AsUtc();
            builder.Property(e => e.DesiredPercentageSplit).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.CurrentPercentageSplit).HasColumnType("DECIMAL(28,18)");

            builder.HasIndex(e => e.CampaignTargetId);

            builder.HasMany(x => x.Lengths).WithOne().HasForeignKey(x => x.CampaignTargetStrikeWeightId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.DayParts).WithOne().HasForeignKey(x => x.CampaignTargetStrikeWeightId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
