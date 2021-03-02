using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BRS
{
    public class BRSConfigurationForKPIEntityConfiguration : IEntityTypeConfiguration<BRSConfigurationForKPI>
    {
        public void Configure(EntityTypeBuilder<BRSConfigurationForKPI> builder)
        {
            builder.ToTable("BRSConfigurationForKPIs");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();
            builder.Property(p => p.KPIName).IsRequired().HasMaxLength(50);
            builder.HasIndex(i => i.BRSConfigurationTemplateId);
        }
    }
}
