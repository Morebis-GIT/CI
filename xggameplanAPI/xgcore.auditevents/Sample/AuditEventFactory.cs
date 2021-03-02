using System;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Factory for AuditEvent instances
    /// </summary>
    internal class AuditEventFactory
    {
        private static AuditEvent Create(int tenantId, int userId, string source, int eventTypeId)
        {
            return new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                EventTypeID = eventTypeId,
                TenantID = tenantId,
                UserID = userId,
                Source = source,
                TimeCreated = DateTime.UtcNow
            };
        }

        public static AuditEvent CreateDebugEvent(int tenantId, int userId, string source, string userName, string message, int debugLevel, Exception exception = null)
        {
            AuditEvent auditEvent = Create(tenantId, userId, source, AuditEventTypes.Debug);
            if (!String.IsNullOrEmpty(userName))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.UserName, Value = userName });
            }
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.DebugLevel, Value = debugLevel });
            if (exception != null)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Exception, Value = ExceptionModel.MapFrom(exception) });
            }
            return auditEvent;
        }

        public static AuditEvent CreateAddProgrammeEvent(int tenantId, int userId, string source, string message, Programme programme)
        {
            AuditEvent auditEvent = Create(tenantId, userId, source, AuditEventTypes.AddProgramme);
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Programme, Value = programme });
            return auditEvent;
        }

        public static AuditEvent CreateUpdateProgrammeEvent(int tenantId, int userId, string source, string message, Programme programme)
        {
            AuditEvent auditEvent = Create(tenantId, userId, source, AuditEventTypes.UpdateProgramme);
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Programme, Value = programme });
            return auditEvent;
        }

        public static AuditEvent CreateDeleteProgrammeEvent(int tenantId, int userId, string source, string message, Programme programme)
        {
            AuditEvent auditEvent = Create(tenantId, userId, source, AuditEventTypes.DeleteProgramme);
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Programme, Value = programme });
            return auditEvent;
        }

        public static AuditEvent CreateScheduledProgrammeEvent(int tenantId, int userId, string source, string message, Programme programme, Guid channelId)
        {
            AuditEvent auditEvent = Create(tenantId, userId, source, AuditEventTypes.DeleteProgramme);
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Programme, Value = programme });
            auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.ChannelId, Value = channelId });
            return auditEvent;
        }

        public static AuditEvent CreateTestValuesEvent(int tenantId, int userId, string source, string message, TestValues testValues)
        {
            AuditEvent auditEvent = Create(tenantId, userId, source, AuditEventTypes.TestValues);
            if (!String.IsNullOrEmpty(message))
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.Message, Value = message });
            }
            if (testValues != null)
            {
                auditEvent.Values.Add(new AuditEventValue() { TypeID = AuditEventValueTypes.TestValues, Value = testValues });
            }
            return auditEvent;
        }
    }
}
