using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Products
{
    public class ProductAdvertiserEntityConfiguration : IEntityTypeConfiguration<ProductAdvertiser>
    {
        public void Configure(EntityTypeBuilder<ProductAdvertiser> builder)
        {
            builder.ToTable("ProductAdvertisers");

            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.EndDate).AsUtc();

            builder.HasOne(p => p.Product).WithMany(p => p.ProductAdvertisers).HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.Advertiser).WithMany().HasForeignKey(p => p.AdvertiserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.ProductId);
            builder.HasIndex(p => p.AdvertiserId);

            builder.HasIndex(p => new
            {
                p.ProductId,
                p.AdvertiserId,
                p.StartDate,
                p.EndDate
            }).IsUnique();
        }
    }
}
