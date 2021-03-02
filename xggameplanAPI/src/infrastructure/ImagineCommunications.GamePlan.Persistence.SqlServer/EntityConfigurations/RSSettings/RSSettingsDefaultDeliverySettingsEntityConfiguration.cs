using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.RSSettings
{
    public class RSSettingsDefaultDeliverySettingsEntityConfiguration : IEntityTypeConfiguration<RSSettingsDefaultDeliverySettings>
    {
        public void Configure(EntityTypeBuilder<RSSettingsDefaultDeliverySettings> builder)
        {
            builder.ToTable("RSSettingsDefaultDeliverySettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.RSSettingId);
            builder.Property(e => e.DaysToCampaignEnd);
            builder.Property(e => e.LowerLimitOfOverDelivery);
            builder.Property(e => e.UpperLimitOfOverDelivery);

            builder.HasIndex(x => x.RSSettingId);
        }
    }
}
