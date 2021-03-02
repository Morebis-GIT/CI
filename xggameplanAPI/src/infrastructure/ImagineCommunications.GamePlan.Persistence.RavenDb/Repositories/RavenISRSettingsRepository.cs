using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenISRSettingsRepository : IISRSettingsRepository
    {
        private readonly IDocumentSession _session;

        public RavenISRSettingsRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(IEnumerable<ISRSettings> items)
        {
            lock (_session)
            {
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
                {
                    items.ToList().ForEach(item => _session.Store(item));
                }
            }
        }

        public ISRSettings Find(string salesArea)
        {
            lock (_session)
            {
                return _session.Query<ISRSettings>().Where(s => s.SalesArea == salesArea).FirstOrDefault();
            }
        }

        public IEnumerable<ISRSettings> FindBySalesAreas(IEnumerable<string> salesAreas) => _session.GetAll<ISRSettings>(x => x.SalesArea.In(salesAreas));

        public List<ISRSettings> GetAll()
        {
            lock (_session)
            {
                return _session.Query<ISRSettings>().Take(Int32.MaxValue).ToList();
            }
        }

        public void Update(ISRSettings isrSettings)
        {
            lock (_session)
            {
                _session.Store(isrSettings);
            }
        }

        public void Delete(string salesArea)
        {
            lock (_session)
            {
                ISRSettings isrSettings = _session.Query<ISRSettings>().Where(s => s.SalesArea == salesArea).First();
                _session.Delete<ISRSettings>(isrSettings);
            }
        }

        public void SaveChanges()
        {
        }
    }
}
