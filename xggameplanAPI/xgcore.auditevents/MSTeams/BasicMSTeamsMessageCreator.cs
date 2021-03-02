using System;
using System.Collections.Generic;
using System.Text;
using xggameplan.common.MSTeams;

namespace xggameplan.AuditEvents
{
    public class BasicMSTeamsMessageCreator : IMSTeamsMessageCreator
    {
        private MSTeamsREST _msTeamsREST;
        private List<MSTeamsAuditEventSettings> _msTeamsAuditEventSettingsList;
        private List<AuditEventValueConverter> _valueConverters;
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;

        public BasicMSTeamsMessageCreator(MSTeamsREST msTeamsREST,
                            List<MSTeamsAuditEventSettings> msTeamsAuditEventSettingsList,
                            List<AuditEventValueConverter> valueConverters,
                            IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            _msTeamsREST = msTeamsREST;
            _msTeamsAuditEventSettingsList = msTeamsAuditEventSettingsList;
            _valueConverters = valueConverters;
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
        }

        public string Id
        {
            get { return "Basic"; }
        }

        /// <summary>
        /// Posts message to MS Teams channel
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="auditEventType"></param>
        /// <param name="postMessageSettings"></param>
        public void PostMessage(AuditEvent auditEvent, AuditEventType auditEventType, MSTeamsPostMessageSettings postMessageSettings)
        {
            StringBuilder title = new StringBuilder(auditEventType.Description);

            StringBuilder message = new StringBuilder(auditEventType.Description);
            foreach (AuditEventValue auditEventValue in auditEvent.Values)
            {
                AuditEventValueType auditEventValueType = _auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                if (auditEventValueType.Type == typeof(String))
                {
                    _ = message.Append("; ");
                    _ = message.Append(string.Format("{0}={1}", auditEventValueType.Description, auditEventValue.Value.ToString()));
                }
            }

            // Post message
            _msTeamsREST.PostSimpleMessage(postMessageSettings.Url, "", message.ToString());
        }

        public bool Handles(AuditEvent auditEvent)
        {
            MSTeamsAuditEventSettings auditEventSettings = _msTeamsAuditEventSettingsList == null ? null : _msTeamsAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID && s.MessageCreatorId == this.Id);
            if (auditEventSettings == null || auditEventSettings.PostMessageSettings == null || !auditEventSettings.PostMessageSettings.Enabled)
            {
                return false;
            }
            return true;
        }
    }
}
