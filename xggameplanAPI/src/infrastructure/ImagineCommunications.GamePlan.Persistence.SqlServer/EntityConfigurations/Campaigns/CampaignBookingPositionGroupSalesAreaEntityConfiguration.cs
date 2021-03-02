using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignBookingPositionGroupSalesAreaEntityConfiguration : IEntityTypeConfiguration<CampaignBookingPositionGroupSalesArea>
    {
        public void Configure(EntityTypeBuilder<CampaignBookingPositionGroupSalesArea> builder)
        {
            builder.ToTable("CampaignBookingPositionGroupSalesAreas");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(TextColumnLenght.Medium).IsRequired();

            builder.HasIndex(e => e.CampaignBookingPositionGroupId);
            builder.HasIndex(e => e.Name);
        }
    }
}
