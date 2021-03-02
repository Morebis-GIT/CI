using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutoBookSettingsRepository : IAutoBookSettingsRepository
    {
        private readonly IDocumentSession _session;
        private readonly IMapper _mapper;

        public RavenAutoBookSettingsRepository(IDocumentSession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public AutoBookSettings Get()
        {
            lock (_session)
            {
                return _session.Query<AutoBookSettings>().FirstOrDefault();
            }
        }

        public void AddOrUpdate(AutoBookSettings autoBookSettings)
        {
            lock (_session)
            {
                var currentSettings = Get();
                if (autoBookSettings == currentSettings)
                {
                    return;
                }
                if (currentSettings != null)
                {
                    _mapper.Map(autoBookSettings, currentSettings);
                    _session.Store(currentSettings);
                }
                else
                {
                    _session.Store(autoBookSettings);
                }
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
