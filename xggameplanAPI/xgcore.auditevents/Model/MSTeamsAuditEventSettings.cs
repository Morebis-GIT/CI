namespace xggameplan.AuditEvents
{
    /*
    public enum MSTeamsActions : short
    {
        PostMessage = 1
    }
    */

    /// <summary>
    /// Audit event settings for MS Teams
    /// </summary>
    public class MSTeamsAuditEventSettings
    {
        /// <summary>
        /// Event type
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Id of IAuditEventEmailCreator that formats messages for this event
        /// </summary>
        public string MessageCreatorId { get; set; }

        /// <summary>
        /// Settings for posting message to MS Teams
        /// </summary>
        public MSTeamsPostMessageSettings PostMessageSettings { get; set; }
    }

    /// <summary>
    /// Settings for posting message to MS Teams
    /// </summary>
    public class MSTeamsPostMessageSettings
    {
        /// <summary>
        /// Whether post is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// URL to post to
        /// </summary>
        public string Url { get; set; }
    }
}
