namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Audit event settings for SQL
    /// </summary>
    public class SQLAuditEventSettings
    {
        public int EventTypeId { get; set; }

        public bool Enabled { get; set; }

        public string ConnectionString { get; set; }
    }
}
