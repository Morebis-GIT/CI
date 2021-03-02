using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.AuditEvents;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenMSTeamsAuditEventSettingsRepository : IMSTeamsAuditEventSettingsRepository
    {
        private readonly IDocumentSession _session;

        public RavenMSTeamsAuditEventSettingsRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Insert(List<MSTeamsAuditEventSettings> items)
        {
            lock (_session)
            {
                using (_session.Advanced.DocumentStore.BulkInsert())
                {
                    items.ToList().ForEach(item => _session.Store(item));
                }
            }
        }

        public List<MSTeamsAuditEventSettings> GetAll()
        {
            lock (_session)
            {
                return _session.Query<MSTeamsAuditEventSettings>().Take(Int32.MaxValue).ToList();
            }
        }

        public void Update(MSTeamsAuditEventSettings auditEventSettings)
        {
            lock (_session)
            {
                _session.Store(auditEventSettings);
            }
        }

        public void DeleteByID(int id)
        {
            lock (_session)
            {
                MSTeamsAuditEventSettings auditEventSettings = _session.Query<MSTeamsAuditEventSettings>().Where(aes => aes.EventTypeId == id).FirstOrDefault();
                if (auditEventSettings != null)
                {
                    _session.Delete(auditEventSettings);
                }
            }
        }

        public void DeleteAll()
        {
            lock (_session)
            {
                _session.TryDelete("Raven/DocumentsByEntityName", "MSTeamsAuditEventSettings");
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
