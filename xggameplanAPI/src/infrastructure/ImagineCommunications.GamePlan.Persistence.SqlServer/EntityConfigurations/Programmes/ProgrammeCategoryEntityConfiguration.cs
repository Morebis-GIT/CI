using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Programmes
{
    public class ProgrammeCategoryEntityConfiguration : IEntityTypeConfiguration<ProgrammeCategory>
    {
        public void Configure(EntityTypeBuilder<ProgrammeCategory> builder)
        {
            _ = builder.ToTable("ProgrammeCategories");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(x => x.Name).HasMaxLength(64).IsRequired();

            _ = builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}
