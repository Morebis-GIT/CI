using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgCampaignProgrammeEntityConfiguration : IEntityTypeConfiguration<AgCampaignProgramme>
    {
        public void Configure(EntityTypeBuilder<AgCampaignProgramme> builder)
        {
            builder.ToTable("AgCampaignProgrammes");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);
            builder.Property(e => e.EndDate).HasMaxLength(32);

            builder.Property(e => e.SalesAreas).AsDelimitedString();

            builder.HasMany(e => e.CategoryOrProgramme).WithOne()
                .HasForeignKey(e => e.AgCampaignProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.TimeBands).WithOne()
                .HasForeignKey(e => e.AgCampaignProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.AutoBookDefaultParametersId);
        }
    }
}
