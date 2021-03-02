using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ProductSettingFeatureEntityConfiguration : IEntityTypeConfiguration<ProductFeature>
    {
        public virtual void Configure(EntityTypeBuilder<ProductFeature> builder)
        {
            builder.ToTable("ProductSettingFeatures");

            builder.HasKey(k => k.Id);

            builder.Property(e => e.Name).HasMaxLength(128);
            builder.Property(e => e.Enabled).HasDefaultValue(false).IsRequired();
            builder.Property(e => e.Settings).HasMaxLength(2048).HasDefaultValue("{}");

            builder.HasIndex(i => i.ProductSettingsId);
        }
    }
}
