namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Filter for audit event
    /// </summary>
    public class AuditEventValueFilter
    {
        public int ValueTypeID { get; set; }

        public object Value { get; set; }
    }
}
