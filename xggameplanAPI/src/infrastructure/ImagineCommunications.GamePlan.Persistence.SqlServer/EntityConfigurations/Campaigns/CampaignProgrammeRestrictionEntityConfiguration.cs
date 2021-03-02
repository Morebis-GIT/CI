using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignProgrammeRestrictionEntityConfiguration : IEntityTypeConfiguration<CampaignProgrammeRestriction>
    {
        public void Configure(EntityTypeBuilder<CampaignProgrammeRestriction> builder)
        {
            builder.ToTable("CampaignProgrammeRestrictions");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.HasIndex(e => e.CampaignId);

            builder.HasMany(x => x.CategoryOrProgramme).WithOne().HasForeignKey(x => x.CampaignProgrammeRestrictionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.SalesAreas).WithOne().HasForeignKey(x => x.CampaignProgrammeRestrictionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
