using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenBRSConfigurationTemplateRepository : IBRSConfigurationTemplateRepository
    {
        private readonly IDocumentSession _session;

        public RavenBRSConfigurationTemplateRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(BRSConfigurationTemplate item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Update(BRSConfigurationTemplate item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item != null)
                {
                    _session.Delete(item);
                }
            }
        }

        public BRSConfigurationTemplate Get(int id) => _session.Load<BRSConfigurationTemplate>(id);

        public BRSConfigurationTemplate GetByName(string name) => _session.Query<BRSConfigurationTemplate>().FirstOrDefault(x => x.Name == name);

        public IEnumerable<BRSConfigurationTemplate> GetAll() => _session.GetAll<BRSConfigurationTemplate>();

        public BRSConfigurationTemplate GetDefault() => _session.Query<BRSConfigurationTemplate>().FirstOrDefault(x => x.IsDefault);

        public bool ChangeDefaultConfiguration(int id)
        {
            var newDefault = _session.Load<BRSConfigurationTemplate>(id);
            if (newDefault == null)
            {
                return false;
            }

            var oldDefault = GetDefault();
            oldDefault.IsDefault = false;
            newDefault.IsDefault = true;
            return true;
        }

        public int Count() => _session.CountAll<BRSConfigurationTemplate>();

        public bool Exists(int id) => Get(id) != null;

        public void SaveChanges() => _session.SaveChanges();
    }
}
