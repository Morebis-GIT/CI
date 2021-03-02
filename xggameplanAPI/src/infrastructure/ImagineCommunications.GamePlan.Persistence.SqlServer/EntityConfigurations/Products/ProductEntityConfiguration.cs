using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Products
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).HasColumnName("uid");

            builder.Property(p => p.Externalidentifier).HasMaxLength(64);
            builder.Property(p => p.ParentExternalidentifier).HasMaxLength(64);
            builder.Property(p => p.Name).HasMaxLength(255);
            builder.Property(p => p.ClashCode).HasMaxLength(64);
            builder.Property(p => p.ReportingCategory).HasMaxLength(256);

            builder.HasFtsField(Product.SearchFieldName, Product.SearchFieldSources);

            builder.HasIndex(p => p.ParentExternalidentifier);
            builder.HasIndex(p => p.ClashCode);
            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.Externalidentifier);
        }
    }
}
