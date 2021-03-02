using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class RunEventSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public Guid TenantSettingsId { get; set; }

        public int EventType { get; set; }
        
        public EmailNotificationSettings Email { get; set; }

        public HTTPNotificationSettings HTTP { get; set; }
    }
}
