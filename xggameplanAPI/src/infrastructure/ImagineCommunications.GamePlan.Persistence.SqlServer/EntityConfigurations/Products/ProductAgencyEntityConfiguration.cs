using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Products
{
    public class ProductAgencyEntityConfiguration : IEntityTypeConfiguration<ProductAgency>
    {
        public void Configure(EntityTypeBuilder<ProductAgency> builder)
        {
            builder.ToTable("ProductAgencies");

            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.EndDate).AsUtc();

            builder.HasOne(p => p.Product).WithMany(p => p.ProductAgencies).HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.Agency).WithMany().HasForeignKey(p => p.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.AgencyGroup).WithMany().HasForeignKey(p => p.AgencyGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.ProductId);
            builder.HasIndex(p => p.AgencyId);
            builder.HasIndex(p => p.AgencyGroupId);

            builder.HasIndex(p => new
            {
                p.ProductId,
                p.AgencyId,
                p.AgencyGroupId,
                p.StartDate,
                p.EndDate
            }).IsUnique();
        }
    }
}
