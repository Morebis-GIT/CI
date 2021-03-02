using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using xggameplan.common.Email;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Email audit event repository
    /// </summary>
    public class EmailAuditEventRepository : IAuditEventRepository
    {
        private IEmailConnection _emailConnection;
        private List<IAuditEventEmailCreator> _emailCreators = new List<IAuditEventEmailCreator>();

        public EmailAuditEventRepository(IEmailConnection emailConnection,
                                         List<IAuditEventEmailCreator> emailCreators)
        {
            _emailConnection = emailConnection;
            _emailCreators = emailCreators;
        }

        public void Insert(AuditEvent auditEvent)
        {
            IAuditEventEmailCreator emailCreator = _emailCreators.Where(ec => ec.Handles(auditEvent)).FirstOrDefault();
            if (emailCreator != null)
            {
                MailMessage mailMessage = emailCreator.CreateEmail(auditEvent);
                if (mailMessage != null)
                {
                    _emailConnection.SendEmail(mailMessage);
                }
            }
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
