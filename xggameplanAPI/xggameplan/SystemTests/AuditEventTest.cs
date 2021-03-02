using System.Collections.Generic;
using System.Linq;
using xggameplan.AuditEvents;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Test audit event mechanism
    /// </summary>
    internal class AuditEventTest : ISystemTest
    {
        private IAuditEventRepository _auditEventRepository;
        private const string _category = "Event Logging";

        public AuditEventTest(IAuditEventRepository auditEventRepository)
        {
            _auditEventRepository = auditEventRepository;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return true;
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();

            try
            {
                // Execute logging persistence check
                results.AddRange(ExecuteLoggingTest());                
            }
            catch(System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error testing event logging: {0}", exception.Message), ""));
            }            
            return results;
        }       

        private List<SystemTestResult> ExecuteLoggingTest()
        {
            var results = new List<SystemTestResult>();

            // Write audit event
            AuditEvent auditEvent = AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Test message");
            _auditEventRepository.Insert(auditEvent);

            // Try reading audit event, may fail if there's an issue such as log file permissions
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = auditEvent.TimeCreated.AddSeconds(-1),
                MaxTimeCreated = auditEvent.TimeCreated.AddSeconds(1)
            };
            var auditEvents = _auditEventRepository.Get(auditEventFilter);
            AuditEvent auditEventLogged = auditEvents.Where(ae => ae.ID == auditEvent.ID).FirstOrDefault();
            if (auditEventLogged == null)       // Not logged
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Event logging logging mechanism is not working because it was not possible to log a test event. There may be a problem with log folder permissions or disk space on the Gameplan API server.", ""));
            }
            else
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Event logging mechanism is OK", ""));
            }
            return results;
        }
    }
}