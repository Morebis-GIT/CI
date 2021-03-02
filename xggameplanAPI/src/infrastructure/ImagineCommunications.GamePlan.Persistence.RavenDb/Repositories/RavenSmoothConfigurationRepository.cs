using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSmoothConfigurationRepository : ISmoothConfigurationRepository
    {
        private readonly IDocumentSession _session;

        public RavenSmoothConfigurationRepository(IDocumentSession session)
        {
            _session = session;
        }

        public SmoothConfiguration GetById(int id)
        {
            lock (_session)
            {
                return _session.Load<SmoothConfiguration>(id);
            }
        }

        public void Add(SmoothConfiguration smoothConfiguration)
        {
            lock (_session)
            {
                _session.Store(smoothConfiguration);
            }
        }

        public void Update(SmoothConfiguration smoothConfiguration)
        {
            lock (_session)
            {
                _session.Store(smoothConfiguration);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Truncate() => throw new System.NotImplementedException();
    }
}
