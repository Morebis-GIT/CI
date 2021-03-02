using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PipelineAuditEvents;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.PipelineAuditEvents
{
    class PipelineAuditEventEntityConfiguration : IEntityTypeConfiguration<PipelineAuditEvent>
    {
        public void Configure(EntityTypeBuilder<PipelineAuditEvent> builder)
        {
            builder.ToTable("PipelineAuditEvents");

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.TimeCreated).AsUtc();
            builder.Property(x => x.Message).HasMaxLength(4096).IsRequired();

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.RunId);
            builder.HasIndex(x => x.ScenarioId);
        }
    }
}
