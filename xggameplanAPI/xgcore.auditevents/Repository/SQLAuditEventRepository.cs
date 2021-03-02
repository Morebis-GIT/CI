using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// SQL audit event repository, persists event to SQL database
    /// </summary>
    public class SQLAuditEventRepository : IAuditEventRepository
    {
        private List<SQLAuditEventSettings> _sqlAuditEventSettingsList;

        public SQLAuditEventRepository(List<SQLAuditEventSettings> sqlAuditEventSettingsList)
        {
            _sqlAuditEventSettingsList = sqlAuditEventSettingsList;
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
            throw new NotImplementedException();
        }

        private bool Handles(AuditEvent auditEvent)
        {
            SQLAuditEventSettings auditEventSettings = _sqlAuditEventSettingsList.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);
            return (auditEventSettings != null && auditEventSettings.Enabled);
        }
    }
}
