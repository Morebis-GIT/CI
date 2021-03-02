using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgPartLengthEntityConfiguration : IEntityTypeConfiguration<AgPartLength>
    {
        public void Configure(EntityTypeBuilder<AgPartLength> builder)
        {
            builder.ToTable("AgPartLengths");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);

            builder.HasIndex(e => e.AgCampaignSalesAreaId);
        }
    }
}
