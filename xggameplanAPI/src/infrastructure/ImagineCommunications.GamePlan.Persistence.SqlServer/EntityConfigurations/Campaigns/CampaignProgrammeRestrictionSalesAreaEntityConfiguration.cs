using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignProgrammeRestrictionSalesAreaEntityConfiguration : IEntityTypeConfiguration<CampaignProgrammeRestrictionSalesArea>
    {
        public void Configure(EntityTypeBuilder<CampaignProgrammeRestrictionSalesArea> builder)
        {
            builder.ToTable("CampaignProgrammeRestrictionsSalesAreas");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.CampaignProgrammeRestrictionId);
            builder.HasIndex(e => e.Name);
        }
    }
}
