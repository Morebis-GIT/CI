using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Metadatas
{
    public class MetadataCategoryEntityConfiguration : IEntityTypeConfiguration<MetadataCategory>
    {
        public void Configure(EntityTypeBuilder<MetadataCategory> builder)
        {
            builder.ToTable("MetadataCategories");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Name).HasMaxLength(128).IsRequired();

            builder.HasIndex(e => e.Name).IsUnique();

            builder.HasMany(e => e.MetadataValues).WithOne().HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
