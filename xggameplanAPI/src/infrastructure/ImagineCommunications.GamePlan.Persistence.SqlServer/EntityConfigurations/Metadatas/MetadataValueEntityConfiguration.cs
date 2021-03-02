using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Metadatas
{
    public class MetadataValueEntityConfiguration : IEntityTypeConfiguration<MetadataValue>
    {
        public void Configure(EntityTypeBuilder<MetadataValue> builder)
        {
            builder.ToTable("MetadataValues");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Value).HasMaxLength(128).IsRequired();

            builder.HasIndex(e => e.CategoryId);
        }
    }
}
