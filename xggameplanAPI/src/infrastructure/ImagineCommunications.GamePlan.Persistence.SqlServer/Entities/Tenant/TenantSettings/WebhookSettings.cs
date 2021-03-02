using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class WebhookSettings
    {
        public int Id { get; set; }

        public Guid TenantSettingsId { get; set; }

        public int EventType { get; set; }

        public HTTPNotificationSettings HTTP { get; set; }
    }
}
