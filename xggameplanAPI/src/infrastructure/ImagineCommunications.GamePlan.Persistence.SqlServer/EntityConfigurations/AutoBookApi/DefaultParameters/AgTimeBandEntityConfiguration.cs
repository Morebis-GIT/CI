using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgTimeBandEntityConfiguration : IEntityTypeConfiguration<AgTimeBand>
    {
        public void Configure(EntityTypeBuilder<AgTimeBand> builder)
        {
            builder.ToTable("AgTimeBands");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.StartTime).HasMaxLength(32);
            builder.Property(e => e.EndTime).HasMaxLength(32);

            builder.HasIndex(e => e.AgCampaignProgrammeId);
        }
    }
}
