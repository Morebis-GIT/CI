using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Console audit event repository, writes event to console
    /// </summary>
    public class ConsoleAuditEventRepository : IAuditEventRepository
    {
        private IAuditEventTypeRepository _auditEventTypeRepository;

        public ConsoleAuditEventRepository(IAuditEventTypeRepository auditEventTypeRepository)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
        }

        public void Insert(AuditEvent auditEvent)
        {
            AuditEventType auditEventType = _auditEventTypeRepository.GetByID(auditEvent.EventTypeID);
            string message = auditEventType.Description;
            System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", auditEvent.TimeCreated, message));
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
