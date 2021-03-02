using System;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Code for testing audit events.
    /// </summary>
    public class AuditEventTest
    {
        public static void TestInformationMessage(IAuditEventRepository auditEventRepository)
        {
            AuditEvent auditEvent = AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Test information message from xG Gameplan");
            auditEventRepository.Insert(auditEvent);
        }

        public static void TestWarningMessage(IAuditEventRepository auditEventRepository)
        {
            AuditEvent auditEvent = AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, "Test warning message from xG Gameplan");
            auditEventRepository.Insert(auditEvent);
        }

        //public static void TestSimpleEmail(IAuditEventRepository auditEventRepository)
        //{
        //    Email email = new Email()
        //    {
        //        Subject = "Test email",
        //        Body = "<html><head></head><body>Test email from xG Gameplan</body></html>",
        //        Sender = "nextgen@imaginecommunications.com",
        //        SenderDisplay = "NextGen",
        //        RecipientList = { "chris.fellows@imaginecommunications.com" }
        //    };
        //    AuditEvent auditEvent = AuditEventFactory.CreateAuditEventForSimpleEmail(0, 0, email);
        //    auditEventRepository.Insert(auditEvent);
        //}

        public static void TestException(IAuditEventRepository auditEventRepository)
        {
            try
            {
                // Generate exception
                int divisor = 0;
                int result = 10 / divisor;
            }
            catch (System.Exception exception)
            {
                // Create new exception with other exception as inner exception
                System.Exception exception2 = new System.IO.FileNotFoundException("Test exception", exception);
                exception2.Data.Add("Data1", 10);
                exception2.Data.Add("Data2", DateTime.UtcNow);
                exception2.Data.Add("Data3", "ABC");
                AuditEvent auditEvent2 = AuditEventFactory.CreateAuditEventForException(0, 0, "Test", exception2);
                auditEventRepository.Insert(auditEvent2);
            }
        }
    }
}