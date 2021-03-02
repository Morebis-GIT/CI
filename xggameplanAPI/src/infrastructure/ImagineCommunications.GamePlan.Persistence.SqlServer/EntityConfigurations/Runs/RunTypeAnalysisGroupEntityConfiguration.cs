using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunTypeAnalysisGroupEntityConfiguration : IEntityTypeConfiguration<RunTypeAnalysisGroup>
    {
        public void Configure(EntityTypeBuilder<RunTypeAnalysisGroup> builder)
        {
            builder.ToTable("RunTypeAnalysisGroups");

            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(x => x.KPI).IsRequired().HasMaxLength(128);

            builder.HasOne(p => p.RunType).WithMany(p => p.RunTypeAnalysisGroups).HasForeignKey(p => p.RunTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.AnalysisGroup).WithMany().HasForeignKey(p => p.AnalysisGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.RunTypeId);
            builder.HasIndex(p => p.AnalysisGroupId);

            builder.HasIndex(p => new
            {
                p.RunTypeId,
                p.AnalysisGroupId,
                p.KPI
            }).IsUnique();
        }
    }
}
