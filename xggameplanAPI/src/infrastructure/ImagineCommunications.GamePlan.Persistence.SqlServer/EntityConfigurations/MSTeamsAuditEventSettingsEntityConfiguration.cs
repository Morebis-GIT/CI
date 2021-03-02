using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class MSTeamsAuditEventSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsAuditEventSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsAuditEventSettings> builder)
        {
            builder.ToTable("MSTeamsAuditEventSettings");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.EventTypeId);
            builder.Property(e => e.MessageCreatorId).HasMaxLength(64).IsRequired();

            builder.OwnsOne(
                o => o.PostMessageSettings,
                ba =>
                {
                    ba.Property(e => e.Enabled);
                    ba.Property(e => e.Url).HasMaxLength(256);
                });

            builder.HasIndex(e => e.EventTypeId);
        }
    }
}
