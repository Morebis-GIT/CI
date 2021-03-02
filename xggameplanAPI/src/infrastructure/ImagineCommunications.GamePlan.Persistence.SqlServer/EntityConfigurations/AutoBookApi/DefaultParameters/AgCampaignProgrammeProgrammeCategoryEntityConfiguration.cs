using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgCampaignProgrammeProgrammeCategoryEntityConfiguration : IEntityTypeConfiguration<AgCampaignProgrammeProgrammeCategory>
    {
        public void Configure(EntityTypeBuilder<AgCampaignProgrammeProgrammeCategory> builder)
        {
            builder.ToTable("AgCampaignProgrammeProgrammeCategories");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.AgCampaignProgrammeId);
        }
    }
}
