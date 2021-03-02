using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class TenantProductFeatureReferenceEntityConfiguration : IEntityTypeConfiguration<TenantProductFeatureReference>
    {
        public void Configure(EntityTypeBuilder<TenantProductFeatureReference> builder)
        {
            builder.ToTable("TenantProductFeatureReferences");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).UseSqlServerIdentityColumn();

            builder.Property(f => f.TenantProductFeatureChildId).IsRequired();
            builder.Property(f => f.TenantProductFeatureParentId).IsRequired();

            builder.HasIndex(f => new {f.TenantProductFeatureChildId, f.TenantProductFeatureParentId})
                .IsUnique();
        }
    }
}
