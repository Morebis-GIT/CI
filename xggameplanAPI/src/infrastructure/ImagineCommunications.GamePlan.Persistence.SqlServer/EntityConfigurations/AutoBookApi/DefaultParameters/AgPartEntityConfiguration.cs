using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgPartEntityConfiguration : IEntityTypeConfiguration<AgPart>
    {
        public void Configure(EntityTypeBuilder<AgPart> builder)
        {
            builder.ToTable("AgParts");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);

            builder.HasIndex(e => e.AgCampaignSalesAreaId);
        }
    }
}
