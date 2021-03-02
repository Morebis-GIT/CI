using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using Raven.Client;
using xggameplan.core.Exceptions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenEfficiencySettingsRepository : IEfficiencySettingsRepository
    {
        private readonly IDocumentSession _session;

        public RavenEfficiencySettingsRepository(IDocumentSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Returns default efficiency calculation settings
        /// </summary>
        /// <returns>
        /// Returns default efficiency calculation settings record typeof <see cref="EfficiencySettings"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">When there is more than one efficiency record in data source</exception>
        /// <exception cref="ObjectNotFoundException">When no default efficiency settings found</exception>
        public EfficiencySettings GetDefault()
        {
            return _session.Query<EfficiencySettings>().SingleOrDefault() ??
                   throw new ObjectNotFoundException("There is no efficiency settings record");
        }

        /// <summary>
        /// Updates default efficiency settings record
        /// </summary>
        /// <param name="settings">Payload with latest settings data</param>
        /// <returns> Returns updated settings record </returns>
        /// <exception cref="InvalidOperationException">When there is more than one efficiency record in data source</exception>
        /// <exception cref="ObjectNotFoundException">When no default efficiency settings found</exception>
        public EfficiencySettings UpdateDefault(EfficiencySettings settings)
        {
            if (settings.Id == default)
            {
                var defaultSettings = GetDefault();

                defaultSettings.FulfillFrom(settings);
                _session.Store(defaultSettings);
                return defaultSettings;
            }

            _session.Store(settings);
            return settings;
        }
    }
}
