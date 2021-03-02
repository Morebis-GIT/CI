using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// AuditEvent repository using Windows EventLog
    /// </summary>
    public class EventLogAuditEventRepository : IAuditEventRepository
    {
        private string _logName;
        private string _source;

        public EventLogAuditEventRepository(string logName, string source)
        {
            _logName = logName;
            _source = source;
            try
            {
                if (!EventLog.SourceExists(source))
                {
                    System.Diagnostics.EventLog.CreateEventSource(source, logName);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Insert(AuditEvent auditEvent)
        {
            //using (EventLog eventLog = new EventLog(_logName))
            //{
            //    eventLog.Source = _logName;

            //    StringBuilder message = new StringBuilder("");
            //    EventLogEntryType entryType = EventLogEntryType.Information;

            //    if (auditEvent.EventTypeID == AuditEventTypes.Exception)      // Error
            //    {
            //        entryType = EventLogEntryType.Error;
            //        AuditEventValue messageValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.Message);
            //        AuditEventValue exceptionValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.Exception);
            //        if (messageValue != null)
            //        {
            //            message.AppendLine(messageValue.Value.ToString());
            //        }
            //        if (exceptionValue != null)
            //        {
            //            // Add all exceptions
            //            Exception exception = (Exception)exceptionValue.Value;
            //            int exceptionLevel = 0;
            //            while (exception != null)
            //            {
            //                exceptionLevel++;
            //                if (exceptionLevel > 1)
            //                {
            //                    message.AppendLine("-----------------------------------------------");
            //                }
            //                message.AppendLine(string.Format("Exception {0}", exceptionLevel));
            //                message.AppendLine(string.Format("TenantID={0}, UserID={1}", auditEvent.TenantID, auditEvent.UserID));
            //                message.AppendLine(string.Format("Message={0}", exception.Message));
            //                message.AppendLine(string.Format("Stack={0}", exception.StackTrace));
            //                exception = exception.InnerException;
            //            }
            //        }
            //    }
            //    else if (auditEvent.EventTypeID == AuditEventTypes.WarningMessage)    // Warning
            //    {
            //        entryType = EventLogEntryType.Warning;
            //        AuditEventValue messageValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.Message);
            //        message.Append(messageValue.Value.ToString());
            //    }
            //    else if (auditEvent.EventTypeID == AuditEventTypes.InformationMessage)   // Information
            //    {
            //        entryType = EventLogEntryType.Information;
            //        AuditEventValue messageValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.Message);
            //        message.Append(messageValue.Value.ToString());
            //    }
            //    else if (auditEvent.EventTypeID == AuditEventTypes.xGG_AutoBookRun)   // Information
            //    {
            //        entryType = EventLogEntryType.Information;
            //        AuditEventValue messageValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.Message);
            //        message.Append(messageValue.Value.ToString());
            //    }
            //    eventLog.WriteEntry(message.ToString(), entryType);
            //}
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
