namespace ImagineCommunications.GamePlan.Domain.Shared.System.Settings
{
    /// <summary>
    /// Settings for a run event
    /// </summary>
    public class WebhookSettings
    {
        public WebhookEvents EventType { get; set; }

        /// <summary>
        /// Settings for HTTP notification. E.g. REST API
        /// </summary>
        public HTTPNotificationSettings HTTP { get; set; }
    }
}
