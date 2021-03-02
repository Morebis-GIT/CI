using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Products
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(k => k.Uid);
            builder.Property(e => e.Uid).HasDefaultValueSql("newid()");

            builder.Property(p => p.Externalidentifier).HasMaxLength(64);
            builder.Property(p => p.ParentExternalidentifier).HasMaxLength(64);
            builder.Property(p => p.Name).HasMaxLength(256);
            builder.Property(p => p.ClashCode).HasMaxLength(64);
            builder.Property(p => p.ReportingCategory).HasMaxLength(256);
            builder.Property<string>(Product.SearchFieldName).HasComputedColumnSql("CONCAT_WS(' ', Externalidentifier, Name)");
            //builder.Property<string>(Product.SearchFieldCampaign).HasComputedColumnSql("CONCAT_WS(' ', AdvertiserName, AgencyName, Name)");

            builder.HasIndex(p => p.ParentExternalidentifier);
            builder.HasIndex(p => p.ClashCode);
            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.Externalidentifier);
        }
    }
}
