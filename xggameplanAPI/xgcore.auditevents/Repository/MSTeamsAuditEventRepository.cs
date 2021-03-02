using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Audit event repository for Microsoft Teams. E.g. Post message to channel
    /// </summary>
    public class MSTeamsAuditEventRepository : IAuditEventRepository
    {
        private readonly string _environmentId;
        private readonly List<IMSTeamsMessageCreator> _msTeamsMessageCreators = null;
        private readonly List<MSTeamsAuditEventSettings> _msTeamsAuditEventSettings = null;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository = null;

        public MSTeamsAuditEventRepository(string environmentId,
                            List<MSTeamsAuditEventSettings> msTeamsAuditEventSettings,
                            List<IMSTeamsMessageCreator> msTeamsMessageCreators,
                            IAuditEventTypeRepository auditeventTypeRepository)
        {
            _environmentId = environmentId;
            _msTeamsAuditEventSettings = msTeamsAuditEventSettings;
            _msTeamsMessageCreators = msTeamsMessageCreators;
            _auditEventTypeRepository = auditeventTypeRepository;
        }

        public void Insert(AuditEvent auditEvent)
        {
            if (Handles(auditEvent))
            {
                MSTeamsAuditEventSettings auditEventSettings = _msTeamsAuditEventSettings.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);

                //AuditEventTypeRepository auditEventTypeRepository = new AuditEventTypeRepository();

                // Get event type
                AuditEventType auditEventType = _auditEventTypeRepository.GetByID(auditEvent.EventTypeID);

                // Perform action(s)
                if (auditEventSettings.PostMessageSettings != null && auditEventSettings.PostMessageSettings.Enabled)
                {
                    IMSTeamsMessageCreator messageCreator = _msTeamsMessageCreators.Where(ec => ec.Handles(auditEvent)).FirstOrDefault();
                    messageCreator.PostMessage(auditEvent, auditEventType, auditEventSettings.PostMessageSettings);
                }
            }
        }

        private bool Handles(AuditEvent auditEvent)
        {
            MSTeamsAuditEventSettings auditEventSettings = _msTeamsAuditEventSettings.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);
            return (auditEventSettings != null && auditEventSettings.PostMessageSettings != null && auditEventSettings.PostMessageSettings.Enabled);
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
