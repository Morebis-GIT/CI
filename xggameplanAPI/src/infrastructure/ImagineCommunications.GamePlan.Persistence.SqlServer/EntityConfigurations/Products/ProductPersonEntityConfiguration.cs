using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Products
{
    public class ProductPersonEntityConfiguration : IEntityTypeConfiguration<ProductPerson>
    {
        public void Configure(EntityTypeBuilder<ProductPerson> builder)
        {
            builder.ToTable("ProductPersons");

            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.EndDate).AsUtc();

            builder.HasOne(p => p.Product).WithMany(p => p.ProductPersons).HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.Person).WithMany().HasForeignKey(p => p.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.ProductId);
            builder.HasIndex(p => p.PersonId);

            builder.HasIndex(p => new
            {
                p.ProductId,
                p.PersonId,
                p.StartDate,
                p.EndDate
            }).IsUnique();
        }
    }
}
