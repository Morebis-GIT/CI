using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.RSSettings
{
    public class RSSettingsDemographicsDeliverySettingsEntityConfiguration : IEntityTypeConfiguration<RSSettingsDemographicsDeliverySettings>
    {
        public void Configure(EntityTypeBuilder<RSSettingsDemographicsDeliverySettings> builder)
        {
            builder.ToTable("RSSettingsDemographicsDeliverySettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.RSSettingsDemographicsSettingId);
            builder.Property(e => e.DaysToCampaignEnd);
            builder.Property(e => e.LowerLimitOfOverDelivery);
            builder.Property(e => e.UpperLimitOfOverDelivery);

            builder.HasIndex(x => x.RSSettingsDemographicsSettingId);
        }
    }
}
