using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface to audit event repository
    /// </summary>
    public interface IAuditEventRepository
    {
        /// <summary>
        /// Inserts audit event
        /// </summary>
        /// <param name="auditEvent"></param>
        void Insert(AuditEvent auditEvent);

        /// <summary>
        /// Returns audit events matching criteria. This method is only applicable for repositories that support persistence
        /// </summary>
        /// <param name="auditEventFilter"></param>
        /// <returns></returns>
        List<AuditEvent> Get(AuditEventFilter auditEventFilter);

        /// <summary>
        /// Deletes audit events matching criteria. This method is only applicable for repositories that support persistence.
        /// </summary>
        /// <param name="auditEventFilter"></param>
        void Delete(AuditEventFilter auditEventFilter);
    }
}
