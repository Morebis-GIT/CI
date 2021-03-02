namespace xggameplan.AuditEvents
{
    public interface ITextFileAuditEventFormatter
    {
        string Id { get; }

        string Format(AuditEvent auditEvent);

        bool Handles(AuditEvent auditEvent);
    }
}
