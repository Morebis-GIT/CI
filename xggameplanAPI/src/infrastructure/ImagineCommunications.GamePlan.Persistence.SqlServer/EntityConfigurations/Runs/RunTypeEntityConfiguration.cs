using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunTypeEntityConfiguration : IEntityTypeConfiguration<RunType>
    {
        public void Configure(EntityTypeBuilder<RunType> builder)
        {
            builder.ToTable("RunTypes");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Description).HasMaxLength(512);
            builder.Property(e => e.ModifiedDate).AsUtc();
            builder.Property(x => x.DefaultKPI).HasMaxLength(128);
        }
    }
}
