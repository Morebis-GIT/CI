using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSalesAreaTargetMultipartEntityConfiguration : IEntityTypeConfiguration<CampaignSalesAreaTargetMultipart>
    {
        public void Configure(EntityTypeBuilder<CampaignSalesAreaTargetMultipart> builder)
        {
            builder.ToTable("CampaignSalesAreaTargetMultiparts");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.DesiredPercentageSplit).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.CurrentPercentageSplit).HasColumnType("DECIMAL(28,18)");

            builder.HasIndex(e => e.CampaignSalesAreaTargetId);

            builder.HasMany(x => x.Lengths).WithOne().HasForeignKey(x => x.CampaignSalesAreaTargetMultipartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
