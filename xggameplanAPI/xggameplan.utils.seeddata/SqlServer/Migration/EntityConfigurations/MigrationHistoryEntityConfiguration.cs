using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using xggameplan.utils.seeddata.SqlServer.Migration.Entities;

namespace xggameplan.utils.seeddata.SqlServer.Migration.EntityConfigurations
{
    public class MigrationHistoryEntityConfiguration : IEntityTypeConfiguration<MigrationHistory>
    {
        public void Configure(EntityTypeBuilder<MigrationHistory> builder)
        {
            _ = builder.ToTable("__RavenMigrationHistory");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(e => e.CollectionName).HasMaxLength(128).IsRequired();
            _ = builder.Property(e => e.Date).AsUtc();

            _ = builder.HasIndex(e => e.CollectionName).IsUnique();
        }
    }
}
