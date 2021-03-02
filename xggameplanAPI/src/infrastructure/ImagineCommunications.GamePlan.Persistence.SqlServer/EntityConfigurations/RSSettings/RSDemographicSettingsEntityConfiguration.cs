using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.RSSettings
{
    public class RSDemographicSettingsEntityConfiguration : IEntityTypeConfiguration<RSDemographicSettings>
    {
        public void Configure(EntityTypeBuilder<RSDemographicSettings> builder)
        {
            builder.ToTable("RSSettingsDemographicsSettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.RSSettingId);
            builder.Property(e => e.DemographicId).HasMaxLength(64);

            builder.HasMany(e => e.DeliverySettingsList).WithOne()
                .HasForeignKey(e => e.RSSettingsDemographicsSettingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.RSSettingId);
            builder.HasIndex(x => x.DemographicId);
        }
    }
}
