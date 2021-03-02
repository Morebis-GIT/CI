using System.Net.Mail;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for and audit event email creator, creates email message for audit event
    /// </summary>
    public interface IAuditEventEmailCreator
    {
        /// <summary>
        /// Id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Create email for audit event
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        MailMessage CreateEmail(AuditEvent auditEvent);

        /// <summary>
        /// Whether email can be created for particular audit event
        /// </summary>
        /// <param name="auditEventTypeId"></param>
        /// <returns></returns>
        bool Handles(AuditEvent auditEvent);
    }
}
