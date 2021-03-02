namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Audit event settings for text file
    /// </summary>
    public class TextFileAuditEventSettings
    {
        /// <summary>
        /// Event type
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Id of ITextFileAuditEventFormatter that formats text file messages for this event
        /// </summary>
        public string FormatterId { get; set; }

        /// <summary>
        /// Whether enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}
