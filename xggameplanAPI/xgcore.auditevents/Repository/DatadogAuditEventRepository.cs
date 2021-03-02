using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Datadog audit event repository, passes event to Datadog
    ///
    /// TODO: Remove this if API is REST, can use HTTPAuditEventRepository
    /// </summary>
    public class DatadogAuditEventRepository : IAuditEventRepository
    {
        public DatadogAuditEventRepository()
        {

        }

        public void Insert(AuditEvent auditEvent)
        {
            throw new NotImplementedException();
        }

        public List<AuditEvent> Get(AuditEventFilter auditEventFilter)
        {
            throw new NotImplementedException();
        }

        public void Delete(AuditEventFilter auditEventFilter)
        {
            // No action
        }
    }
}
