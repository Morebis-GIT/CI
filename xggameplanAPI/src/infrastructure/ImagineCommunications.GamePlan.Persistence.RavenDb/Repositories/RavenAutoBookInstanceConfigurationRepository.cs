using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutoBookInstanceConfigurationRepository : IAutoBookInstanceConfigurationRepository
    {
        private readonly IDocumentSession _session;

        public RavenAutoBookInstanceConfigurationRepository(IDocumentSession session) =>
            _session = session;

        public void Add(IEnumerable<AutoBookInstanceConfiguration> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public List<AutoBookInstanceConfiguration> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<AutoBookInstanceConfiguration>();
            }
        }

        public AutoBookInstanceConfiguration Get(int id)
        {
            lock (_session)
            {
                return _session.Load<AutoBookInstanceConfiguration>(id);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
                WaitForIndexes();
            }
        }

        private void WaitForIndexes()
        {
            _session.WaitForIndexes<AutoBookInstanceConfiguration>(
                AutoBookInstanceConfiguration_ByIdAndDescription.DefaultIndexName,
                isMapReduce: false,
                testExpression: p => p.Id != int.MinValue
                );
        }
    }
}
