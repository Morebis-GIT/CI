using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.TenantSettings
{
    public class TenantWebhookSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.TenantSettings.WebhookSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.TenantSettings.WebhookSettings> builder)
        {
                builder.ToTable("TenantWebhookSettings");

                builder.HasKey(res => res.Id);
                builder.Property(res => res.Id).UseMySqlIdentityColumn();

                builder.Property(res => res.TenantSettingsId);
                builder.Property(res => res.EventType);

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
                                action.Property(ms => ms.ContentTemplate).HasColumnName("HTTPNotificationContentTemplate");
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
