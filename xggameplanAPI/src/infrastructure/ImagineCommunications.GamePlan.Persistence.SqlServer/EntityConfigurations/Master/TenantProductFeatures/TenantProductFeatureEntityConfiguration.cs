using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class TenantProductFeatureEntityConfiguration : IEntityTypeConfiguration<TenantProductFeature>
    {
        public void Configure(EntityTypeBuilder<TenantProductFeature> builder)
        {
            builder.ToTable("TenantProductFeatures");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).UseMySqlIdentityColumn();

            builder.Property(f => f.TenantId).IsRequired();
            builder.Property(f => f.Name).HasMaxLength(64).IsRequired();
            builder.Property(f => f.Enabled).HasDefaultValue(false).IsRequired();
            builder.Property(f => f.IsShared).HasDefaultValue(false).IsRequired();

            builder.HasIndex(f => new {f.Name, f.TenantId})
                .IsUnique();

            builder.HasMany(f => f.ParentFeatures).WithOne()
                .HasForeignKey(r => r.TenantProductFeatureChildId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
