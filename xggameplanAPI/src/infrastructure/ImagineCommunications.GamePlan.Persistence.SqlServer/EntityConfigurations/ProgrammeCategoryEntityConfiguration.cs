using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ProgrammeCategoryHierarchyEntityConfiguration : IEntityTypeConfiguration<ProgrammeCategoryHierarchy>
    {
        public void Configure(EntityTypeBuilder<ProgrammeCategoryHierarchy> builder)
        {
            builder.ToTable("ProgrammeCategoryHierarchy");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();
            builder.Property(e => e.ExternalRef).HasMaxLength(64);
            builder.Property(e => e.ParentExternalRef).HasMaxLength(64);
        }
    }
}
