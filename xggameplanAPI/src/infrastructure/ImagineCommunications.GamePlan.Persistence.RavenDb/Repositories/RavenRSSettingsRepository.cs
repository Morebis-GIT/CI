using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRSSettingsRepository : IRSSettingsRepository
    {
        private readonly IDocumentSession _session;

        public RavenRSSettingsRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(IEnumerable<RSSettings> items)
        {
            lock (_session)
            {
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
                {
                    items.ToList().ForEach(item => _session.Store(item));
                }
            }
        }

        public RSSettings Find(string salesArea)
        {
            lock (_session)
            {
                return _session.Query<RSSettings>().Where(s => s.SalesArea == salesArea).FirstOrDefault();
            }
        }

        public IEnumerable<RSSettings> FindBySalesAreas(IEnumerable<string> salesAreas) => _session.GetAll<RSSettings>(x => x.SalesArea.In(salesAreas));

        public List<RSSettings> GetAll()
        {
            lock (_session)
            {
                return _session.Query<RSSettings>().Take(Int32.MaxValue).ToList();
            }
        }

        public void Update(RSSettings rsSettings)
        {
            lock (_session)
            {
                _session.Store(rsSettings);
            }
        }

        public void Delete(string salesArea)
        {
            lock (_session)
            {
                RSSettings rsSettings = _session.Query<RSSettings>().Where(s => s.SalesArea == salesArea).FirstOrDefault();
                if (rsSettings != null)
                {
                    _session.Delete<RSSettings>(rsSettings);
                }
            }
        }

        public void SaveChanges()
        {
            _session.SaveChanges();
        }
    }
}
