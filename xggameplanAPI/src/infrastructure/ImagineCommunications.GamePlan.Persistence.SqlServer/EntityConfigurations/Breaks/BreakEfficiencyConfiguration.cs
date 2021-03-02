using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Breaks
{
    public class BreakEfficiencyConfiguration : IEntityTypeConfiguration<BreakEfficiency>
    {
        public void Configure(EntityTypeBuilder<BreakEfficiency> builder)
        {
            builder.ToTable("BreaksEfficiencies");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(x => x.Demographic).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.BreakId);
            builder.HasIndex(e => e.Demographic);
        }
    }
}
