using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAWSInstanceConfigurationRepository : IAWSInstanceConfigurationRepository
    {
        private readonly IDocumentSession _session;

        public RavenAWSInstanceConfigurationRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(IEnumerable<AWSInstanceConfiguration> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public List<AWSInstanceConfiguration> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<AWSInstanceConfiguration>();
            }
        }

        public AWSInstanceConfiguration Get(int id)
        {
            lock (_session)
            {
                return _session.Load<AWSInstanceConfiguration>(id);
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
