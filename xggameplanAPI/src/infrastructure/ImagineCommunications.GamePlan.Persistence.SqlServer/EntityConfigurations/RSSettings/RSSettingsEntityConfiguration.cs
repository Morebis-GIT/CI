using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.RSSettings
{
    public class RSSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.RSSettings.RSSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.RSSettings.RSSettings> builder)
        {
            _ = builder.ToTable("RSSettings");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasMany(e => e.DemographicsSettings).WithOne()
                .HasForeignKey(e => e.RSSettingId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasMany(e => e.DefaultDeliverySettingsList).WithOne()
                .HasForeignKey(e => e.RSSettingId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
