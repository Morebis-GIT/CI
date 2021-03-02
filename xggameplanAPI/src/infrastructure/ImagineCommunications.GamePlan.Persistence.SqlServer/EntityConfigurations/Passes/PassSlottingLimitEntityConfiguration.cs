using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassSlottingLimitEntityConfiguration : IEntityTypeConfiguration<PassSlottingLimit>
    {
        public void Configure(EntityTypeBuilder<PassSlottingLimit> builder)
        {
            builder.ToTable("PassSlottingLimits");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Demographs).HasColumnName("Demographic").IsRequired().HasMaxLength(256);

            builder.HasIndex(x => x.PassId);
        }
    }
}
