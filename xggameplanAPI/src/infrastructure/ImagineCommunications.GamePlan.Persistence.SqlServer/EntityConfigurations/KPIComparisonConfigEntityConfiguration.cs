using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class KPIComparisonConfigEntityConfiguration : IEntityTypeConfiguration<KPIComparisonConfig>
    {
        public void Configure(EntityTypeBuilder<KPIComparisonConfig> builder)
        {
            builder.ToTable("KPIComparisonConfigs");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.KPIName).HasMaxLength(256);
        }
    }
}
