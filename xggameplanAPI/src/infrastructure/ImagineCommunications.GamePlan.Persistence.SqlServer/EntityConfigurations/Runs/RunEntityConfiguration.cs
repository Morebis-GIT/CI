using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunEntityConfiguration : IEntityTypeConfiguration<Run>
    {
        public void Configure(EntityTypeBuilder<Run> builder)
        {
            builder.ToTable("Runs");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
            builder.Property(p => p.CreatedDateTime).AsUtc();
            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.StartTime).AsTicks();
            builder.Property(p => p.EndDate).AsUtc();
            builder.Property(p => p.EndTime).AsTicks();
            builder.Property(p => p.LastModifiedDateTime).AsUtc();
            builder.Property(p => p.ExecuteStartedDateTime).AsUtc();
            builder.Property(p => p.SmoothDateStart).AsUtc();
            builder.Property(p => p.SmoothDateEnd).AsUtc();
            builder.Property(p => p.ISRDateStart).AsUtc();
            builder.Property(p => p.ISRDateEnd).AsUtc();
            builder.Property(p => p.OptimisationDateStart).AsUtc();
            builder.Property(p => p.OptimisationDateEnd).AsUtc();
            builder.Property(p => p.RightSizerDateStart).AsUtc();
            builder.Property(p => p.RightSizerDateEnd).AsUtc();
            builder.Property(p => p.FailureTypes).AsDelimitedString();
            builder.Property(p => p.CreatedOrExecuteDateTime)
                .HasComputedColumnSql("ISNULL(ExecuteStartedDateTime,CreatedDateTime)");
            builder.Property(p => p.CreatedOrExecuteDateTime).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            builder.Property(p => p.CreatedOrExecuteDateTime).Metadata.BeforeSaveBehavior = PropertySaveBehavior.Ignore;
            builder.Property<string>(Run.SearchField)
                .HasComputedColumnSql("CONCAT_WS(' ', Id, [Description])");

            builder.HasOne(e => e.Author).WithOne().HasForeignKey<RunAuthor>(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.SalesAreaPriorities).WithOne().HasForeignKey(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Campaigns).WithOne().HasForeignKey(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.CampaignsProcessesSettings).WithOne().HasForeignKey(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Scenarios).WithOne().HasForeignKey(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.AnalysisGroupTargets).WithOne().HasForeignKey(p => p.RunId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
