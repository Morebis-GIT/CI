using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.AuditEvents;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenEmailAuditEventSettingsRepository : IEmailAuditEventSettingsRepository
    {
        private readonly IDocumentSession _session;

        public RavenEmailAuditEventSettingsRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void AddRange(IEnumerable<EmailAuditEventSettings> items)
        {
            lock (_session)
            {
                using (var bulkInsert =
                    _session.Advanced.DocumentStore.BulkInsert(null, new BulkInsertOptions {OverwriteExisting = true}))
                {
                    items.ToList().ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public List<EmailAuditEventSettings> GetAll()
        {
            lock (_session)
            {
                return _session.Query<EmailAuditEventSettings>().Take(Int32.MaxValue).ToList();
            }
        }

        public void Update(EmailAuditEventSettings auditEventSettings)
        {
            lock (_session)
            {
                _session.Store(auditEventSettings);
            }
        }

        public void Delete(int eventTypeid)
        {
            lock (_session)
            {
                EmailAuditEventSettings auditEventSettings = _session
                    .Query<EmailAuditEventSettings>()
                    .Where(aes => aes.EventTypeId == eventTypeid)
                    .FirstOrDefault();

                if (auditEventSettings != null)
                {
                    _session.Delete(auditEventSettings);
                }
            }
        }

        public void Truncate()
        {
            lock (_session)
            {
                _session.TryDelete("Raven/DocumentsByEntityName", "EmailAuditEventSettings");
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
