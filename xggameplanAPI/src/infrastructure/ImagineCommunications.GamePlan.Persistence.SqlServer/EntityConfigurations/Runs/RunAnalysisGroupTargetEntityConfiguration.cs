using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunAnalysisGroupTargetEntityConfiguration : IEntityTypeConfiguration<RunAnalysisGroupTarget>
    {
        public void Configure(EntityTypeBuilder<RunAnalysisGroupTarget> builder)
        {
            builder.ToTable("RunAnalysisGroupTargets");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            builder.Property(p => p.KPI).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.RunId);
            builder.HasIndex(e => e.AnalysisGroupTargetId).IsUnique();
        }
    }
}
