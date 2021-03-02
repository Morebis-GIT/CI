namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Audit event settings for CSV
    /// </summary>
    public class CSVAuditEventSettings
    {
        /// <summary>
        /// Event type
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Whether enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}
