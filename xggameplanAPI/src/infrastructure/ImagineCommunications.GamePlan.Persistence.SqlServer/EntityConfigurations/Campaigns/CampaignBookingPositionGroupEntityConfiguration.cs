using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignBookingPositionGroupEntityConfiguration : IEntityTypeConfiguration<CampaignBookingPositionGroup>
    {
        public void Configure(EntityTypeBuilder<CampaignBookingPositionGroup> builder)
        {
            builder.ToTable("CampaignBookingPositionGroups");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.DesiredPercentageSplit).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.CurrentPercentageSplit).HasColumnType("DECIMAL(28,18)");

            builder.HasIndex(e => e.CampaignId);

            builder.HasMany(x => x.SalesAreas).WithOne()
                .HasForeignKey(x => x.CampaignBookingPositionGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
