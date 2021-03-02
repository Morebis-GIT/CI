using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class EmailAuditEventSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int EventTypeId { get; set; }

        public string EmailCreatorId { get; set; }

        public EmailNotificationSettings NotificationSettings { get; set; }
    }
}
