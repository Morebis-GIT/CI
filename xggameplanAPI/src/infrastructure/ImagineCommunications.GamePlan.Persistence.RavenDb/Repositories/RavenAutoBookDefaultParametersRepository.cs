using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutoBookDefaultParametersRepository : IAutoBookDefaultParametersRepository
    {
        private readonly IDocumentSession _session;

        public RavenAutoBookDefaultParametersRepository(IDocumentSession session) => _session = session;

        public IAutoBookDefaultParameters Get()
        {
            lock (_session)
            {
                return _session.GetAll<AutoBookDefaultParameters>().Single();
            }
        }

        public void AddOrUpdate(IAutoBookDefaultParameters autoBookDefaultParameters)
        {
            if (autoBookDefaultParameters is null)
            {
                return;
            }

            lock (_session)
            {
                _session.Store(autoBookDefaultParameters);
            }
        }

        public void SaveChanges() => _session.SaveChanges();
    }
}
