using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class EmailAuditEventSettingsEntityConfiguration : IEntityTypeConfiguration<EmailAuditEventSettings>
    {
        public void Configure(EntityTypeBuilder<EmailAuditEventSettings> builder)
        {
            builder.ToTable("EmailAuditEventSettings");

            builder.HasKey(res => res.Id);
            builder.Property(res => res.Id).UseMySqlIdentityColumn();

            builder.Property(res => res.EventTypeId);
            builder.Property(res => res.EmailCreatorId).HasMaxLength(64);

            builder.OwnsOne(
                res => res.NotificationSettings,
                act =>
                {
                    act.Property(email => email.Enabled);
                    act.Property(email => email.SenderAddress)
                        .HasMaxLength(256);
                    act.Property(email => email.CCAddresses)
                        .AsDelimitedString();
                    act.Property(email => email.RecipientAddresses)
                        .AsDelimitedString();
                }
            );
            builder.HasIndex(e => e.EventTypeId);
        }
    }
}
