using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class EfficiencySettingsEntityConfiguration : IEntityTypeConfiguration<EfficiencySettings>
    {
        public void Configure(EntityTypeBuilder<EfficiencySettings> builder)
        {
            builder.ToTable("EfficiencySettings");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).HasDefaultValueSql("newid()");

            builder.Property(p => p.EfficiencyCalculationPeriod).IsRequired();
            builder.Property(p => p.PersistEfficiency).IsRequired();
        }
    }
}
