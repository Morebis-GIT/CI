using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignBreakTypeEntityConfiguration : IEntityTypeConfiguration<CampaignBreakType>
    {
        public void Configure(EntityTypeBuilder<CampaignBreakType> builder)
        {
            builder.ToTable("CampaignBreakTypes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.CampaignId);
        }
    }
}
