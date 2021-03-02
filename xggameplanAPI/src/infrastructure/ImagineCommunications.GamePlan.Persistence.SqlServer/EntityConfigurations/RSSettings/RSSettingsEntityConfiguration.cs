using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.RSSettings
{
    public class RSSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.RSSettings.RSSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.RSSettings.RSSettings> builder)
        {
            builder.ToTable("RSSettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SalesArea).HasMaxLength(64);

            builder.HasMany(e => e.DemographicsSettings).WithOne()
                .HasForeignKey(e => e.RSSettingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.DefaultDeliverySettingsList).WithOne()
                .HasForeignKey(e => e.RSSettingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SalesArea);
        }
    }
}
