using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.TenantSettings
{
    public class TenantFeatureSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.TenantSettings.Feature>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.TenantSettings.Feature> builder)
        {
            builder.ToTable("TenantFeatureSettings");

            builder.HasKey(res => res.Id);
            builder.Property(res => res.Id).UseSqlServerIdentityColumn();

            builder.Property(res => res.TenantSettingsId);
            builder.Property(res => res.IdValue).HasMaxLength(256);
            builder.Property(res => res.Enabled);
            builder.Property(res => res.Settings).AsDelimitedString();

            builder.HasIndex(x => x.TenantSettingsId);
        }
    }
}
