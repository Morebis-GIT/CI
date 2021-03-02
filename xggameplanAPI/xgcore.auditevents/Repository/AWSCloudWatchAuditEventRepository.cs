using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// AWS Cloud Watch audit event repository, passes event to AWS Cloud Watch
    ///
    /// TODO: Remove this if API is REST, can use HTTPAuditEventRepository
    /// </summary>
    public class AWSCloudWatchAuditEventRepository : IAuditEventRepository
    {
        public AWSCloudWatchAuditEventRepository()
        {

        }

        public void Insert(AuditEvent auditEvent)
        {

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
