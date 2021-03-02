using System;
using System.Collections.Generic;
using System.Text;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Basic formatter for events for text file.
    /// </summary>
    public class BasicTextFileAuditEventFormatter : ITextFileAuditEventFormatter
    {
        private string _delimiter;
        private List<TextFileAuditEventSettings> _textFileAuditEventSettingsList;
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;

        public BasicTextFileAuditEventFormatter(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository, List<TextFileAuditEventSettings> textFileAuditEventSettingsList, string delimiter)
        {
            _delimiter = delimiter;
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _textFileAuditEventSettingsList = textFileAuditEventSettingsList;
        }

        public string Id
        {
            get { return "Basic"; }
        }

        public string Format(AuditEvent auditEvent)
        {
            AuditEventType auditEventType = _auditEventTypeRepository.GetByID(auditEvent.EventTypeID);
            StringBuilder serialized = new StringBuilder(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}", _delimiter, auditEvent.ID, auditEvent.TimeCreated.ToString("o"), auditEvent.TenantID, auditEvent.UserID, auditEvent.Source, auditEventType.Description));
            int valueCount = 0;
            foreach (AuditEventValue auditEventValue in auditEvent.Values)
            {
                AuditEventValueType auditEventValueType = _auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                if (auditEventValueType.Type == typeof(String))
                {
                    valueCount++;
                    if (valueCount > 1)
                    {
                        _ = serialized.Append("; ");
                    }
                    _ = serialized.Append(string.Format("{0}={1}", auditEventValueType.Description, auditEventValue.Value.ToString()));
                }
            }
            return serialized.ToString();
        }

        public bool Handles(AuditEvent auditEvent)
        {
            TextFileAuditEventSettings auditEventSettings = _textFileAuditEventSettingsList == null ? null : _textFileAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID && s.FormatterId == this.Id);
            if (auditEventSettings == null || !auditEventSettings.Enabled)
            {
                return false;
            }
            return true;
        }
    }
}
