using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ProductSettingsEntityConfiguration : IEntityTypeConfiguration<ProductSettings>
    {
        public virtual void Configure(EntityTypeBuilder<ProductSettings> builder)
        {
            builder.ToTable("ProductSettings");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Description).HasMaxLength(1024);
            
            builder.HasMany(e => e.Features)
                .WithOne()
                .HasForeignKey(e => e.ProductSettingsId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
