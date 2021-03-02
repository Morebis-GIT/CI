using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignTargetStrikeWeightDayPartEntityConfiguration : IEntityTypeConfiguration<CampaignTargetStrikeWeightDayPart>
    {
        public void Configure(EntityTypeBuilder<CampaignTargetStrikeWeightDayPart> builder)
        {
            builder.ToTable("CampaignTargetStrikeWeightDayParts");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.DayPartName).HasMaxLength(256);
            builder.Property(e => e.DesiredPercentageSplit).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.CurrentPercentageSplit).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.CampaignPrice).HasColumnType("DECIMAL(28,18)");

            builder.HasIndex(e => e.CampaignTargetStrikeWeightId);

            builder.HasMany(x => x.Lengths).WithOne().HasForeignKey(x => x.CampaignTargetStrikeWeightDayPartId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Timeslices).WithOne().HasForeignKey(x => x.CampaignTargetStrikeWeightDayPartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
