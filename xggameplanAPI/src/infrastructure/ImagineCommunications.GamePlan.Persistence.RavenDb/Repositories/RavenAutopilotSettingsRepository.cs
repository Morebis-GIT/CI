using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutopilotSettingsRepository : IAutopilotSettingsRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenAutopilotSettingsRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(AutopilotSettings autopilotSettings)
        {
            lock (_session)
            {
                _session.Store(autopilotSettings);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public AutopilotSettings Get(int id) => _session.Load<AutopilotSettings>(id);

        public IEnumerable<AutopilotSettings> GetAll() => _session.GetAll<AutopilotSettings>();

        public AutopilotSettings GetDefault() => _session.GetAll<AutopilotSettings>().FirstOrDefault();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(AutopilotSettings autopilotSettings)
        {
            lock (_session)
            {
                _session.Store(autopilotSettings);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
