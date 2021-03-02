using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookTaskReports;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.PipelineAuditEvents
{
    class AutoBookTaskReportEntityConfiguration : IEntityTypeConfiguration<AutoBookTaskReport>
    {
        public void Configure(EntityTypeBuilder<AutoBookTaskReport> builder)
        {
            builder.ToTable("AutoBookTaskReports");

            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
            builder.Property(x => x.TimeCreated).AsUtc();
            builder.Property(x => x.BinariesVersion).HasMaxLength(64).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(64).IsRequired();
            builder.Property(x => x.InstanceType).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Region).HasMaxLength(64).IsRequired();
            builder.Property(x => x.StorageSizeGB).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Version).HasMaxLength(64).IsRequired();

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.RunId);
            builder.HasIndex(x => x.ScenarioId);
        }
    }
}
