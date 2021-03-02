using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Failures
{
    public class FailureItemEntityConfiguration : IEntityTypeConfiguration<FailureItem>
    {
        public void Configure(EntityTypeBuilder<FailureItem> builder)
        {
            builder.ToTable("FailureItems");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.CampaignName).HasMaxLength(256);
            builder.Property(e => e.ExternalId).HasMaxLength(256);
            builder.Property(e => e.SalesAreaName).HasMaxLength(512);

            builder.HasIndex(e => e.FailureId);
        }
    }
}
