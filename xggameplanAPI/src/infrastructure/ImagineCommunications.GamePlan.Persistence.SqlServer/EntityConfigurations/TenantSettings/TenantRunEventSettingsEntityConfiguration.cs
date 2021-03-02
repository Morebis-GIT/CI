using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.TenantSettings
{
    public class TenantRunEventSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.TenantSettings.RunEventSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.TenantSettings.RunEventSettings> builder)
        {
                builder.ToTable("TenantRunEventSettings");

                builder.HasKey(res => res.Id);
                builder.Property(res => res.Id).UseSqlServerIdentityColumn();

                builder.Property(res => res.TenantSettingsId);
                builder.Property(res => res.EventType).HasColumnName("EventType");

                builder.OwnsOne(
                    res => res.Email,
                    act =>
                    {
                        act.Property(email => email.Enabled)
                            .HasColumnName("EmailNotificationEnabled");
                        act.Property(email => email.SenderAddress)
                            .HasColumnName("EmailNotificationSenderAddress")
                            .HasMaxLength(256);
                        act.Property(email => email.CCAddresses)
                            .HasColumnName("EmailNotificationCCAddresses")
                            .AsDelimitedString();
                        act.Property(email => email.RecipientAddresses)
                            .HasColumnName("EmailNotificationRecipientAddresses")
                            .AsDelimitedString();
                    }
                );

                builder.OwnsOne(
                    res => res.HTTP,
                    act =>
                    {
                        act.Property(http => http.Enabled).HasColumnName("HTTPNotificationEnabled");
                        act.Property(http => http.SucccessStatusCodes)
                            .HasColumnName("HTTPNotificationSucccessStatusCodes")
                            .AsDelimitedString();

                        act.OwnsOne(
                            http => http.MethodSettings,
                            action =>
                            {
                                action.Property(ms => ms.Method)
                                .HasColumnName("HTTPNotificationMethod")
                                .HasMaxLength(16);
                                action.Property(ms => ms.URL).HasColumnName("HTTPNotificationURL");
                                action.Property(ms => ms.ContentTemplate)
                                    .HasColumnName("HTTPNotificationContentTemplate");
                                action.Property(http => http.Headers)
                                    .HasColumnName("HTTPNotificationHeaders")
                                    .AsDelimitedString();
                            }
                        );
                    }
                );

            builder.HasIndex(x => x.TenantSettingsId);
        }
    }
}
