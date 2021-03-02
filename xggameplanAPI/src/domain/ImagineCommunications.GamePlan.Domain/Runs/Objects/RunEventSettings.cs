using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    /// <summary>
    /// Settings for a run event
    /// </summary>
    public class RunEventSettings
    {
        public RunEvents EventType { get; set; }

        /// <summary>
        /// Settings for email notification
        /// </summary>
        public EmailNotificationSettings Email { get; set; }

        /// <summary>
        /// Settings for HTTP notification. E.g. REST API
        /// </summary>
        public HTTPNotificationSettings HTTP { get; set; }
    }
}
