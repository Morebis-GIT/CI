using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Creates tests events
    /// </summary>
    internal class EventCreation
    {
        /// <summary>
        /// Creates a number of test events.
        ///
        /// Normally then events can be created as follows:
        /// - Clients can POST and event to the REST API. The REST API inserts the event in to MasterAuditEventRepository (IAuditEventRepository).
        /// - REST API can generate an event itself by inserting the event in to MasterAuditEventRepository (IAuditEventRepository).
        /// </summary>
        /// <param name="auditEventRepository"></param>
        /// <param name="source"></param>
        public void CreateEvents(IAuditEventRepository auditEventRepository, string source)
        {
            int tenantId = 0;
            int userId = 0;
            string userName = "Joe Bloggs";

            // Debug messages
            auditEventRepository.Insert(AuditEventFactory.CreateDebugEvent(tenantId, userId, source, userName, "This is a diagnostic message", DebugLevels.Diagnostic));
            auditEventRepository.Insert(AuditEventFactory.CreateDebugEvent(tenantId, userId, source, userName, "This is an information message", DebugLevels.Information));
            auditEventRepository.Insert(AuditEventFactory.CreateDebugEvent(tenantId, userId, source, userName, "This is a warning message", DebugLevels.Warning));

            // Exception
            try
            {
                int x = 10;
                int y = 0;
                int result = x / y;
            }
            catch (System.Exception exception)
            {
                auditEventRepository.Insert(AuditEventFactory.CreateDebugEvent(tenantId, userId, source, userName, "This is an exception message", DebugLevels.Exception, new Exception("Top level exception", exception)));
            }

            // Programme add/update/delete
            Programme programme = new Programme()
            {
                Id = Guid.NewGuid(),
                Name = "Top Gear",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMinutes(60)
            };
            auditEventRepository.Insert(AuditEventFactory.CreateAddProgrammeEvent(tenantId, userId, source, "Programme added", programme));
            programme.StartDate = programme.StartDate.AddDays(1);
            programme.EndDate = programme.EndDate.AddDays(1);
            auditEventRepository.Insert(AuditEventFactory.CreateUpdateProgrammeEvent(tenantId, userId, source, "Programme updated", programme));
            auditEventRepository.Insert(AuditEventFactory.CreateDeleteProgrammeEvent(tenantId, userId, source, "Programme deleted", programme));

            // Programme scheduled
            Guid channelId = Guid.NewGuid();
            auditEventRepository.Insert(AuditEventFactory.CreateScheduledProgrammeEvent(tenantId, userId, source, "Programme scheduled", programme, channelId));

            // System state
            TestValues testValues = new TestValues() { Values = new Dictionary<string, object>() };
            testValues.Values.Add("Time", DateTime.UtcNow);
            testValues.Values.Add("Boolean", true);
            testValues.Values.Add("GUID", Guid.NewGuid());
            testValues.Values.Add("Int16", Int16.MaxValue);
            testValues.Values.Add("Int32", Int32.MaxValue);
            testValues.Values.Add("Int64", Int64.MaxValue);
            testValues.Values.Add("UInt16", UInt16.MaxValue);
            testValues.Values.Add("UInt32", UInt32.MaxValue);
            testValues.Values.Add("UInt64", UInt64.MaxValue);
            testValues.Values.Add("Double", Double.MaxValue);
            testValues.Values.Add("Single", Single.MaxValue);
            testValues.Values.Add("Float", float.MaxValue);
            testValues.Values.Add("String", "This is a string value");
            auditEventRepository.Insert(AuditEventFactory.CreateTestValuesEvent(tenantId, userId, source, "These are test values", testValues));
        }
    }
}
