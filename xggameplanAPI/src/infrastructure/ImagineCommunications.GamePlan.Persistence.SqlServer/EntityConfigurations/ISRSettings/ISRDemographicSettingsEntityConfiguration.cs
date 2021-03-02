using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ISRSettings
{
    public class ISRDemographicSettingsEntityConfiguration : IEntityTypeConfiguration<ISRDemographicSettings>
    {
        public void Configure(EntityTypeBuilder<ISRDemographicSettings> builder)
        {
            builder.ToTable("ISRSettingsDemographics");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.ISRSettingId);
            builder.Property(e => e.DemographicId).HasMaxLength(64);
            builder.Property(e => e.EfficiencyThreshold);

            builder.HasIndex(x => x.ISRSettingId);
            builder.HasIndex(x => x.DemographicId);
        }
    }
}
