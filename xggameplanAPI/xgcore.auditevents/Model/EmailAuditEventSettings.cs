using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Settings for audit event that is inserted in to email repository
    /// </summary>
    public class EmailAuditEventSettings
    {
        /// <summary>
        /// Event type
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Id of IAuditEventEmailCreator that formats messages for this event
        /// </summary>
        public string EmailCreatorId { get; set; }

        /// <summary>
        /// Notification settings
        /// </summary>
        public EmailNotificationSettings NotificationSettings { get; set; }
    }
}
