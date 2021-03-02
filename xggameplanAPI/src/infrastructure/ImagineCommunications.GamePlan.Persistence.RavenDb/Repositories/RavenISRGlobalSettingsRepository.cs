using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenISRGlobalSettingsRepository : IISRGlobalSettingsRepository
    {
        private readonly IRavenTenantDbContext _dbContext;

        public RavenISRGlobalSettingsRepository(IRavenTenantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns ISR global settings
        /// </summary>
        /// <returns>
        /// Returns ISR global settings record typeof <see cref="ISRGlobalSettings"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">When there is more than one record in data source</exception>
        public ISRGlobalSettings Get()
        {
            return _dbContext.Query<ISRGlobalSettings>().SingleOrDefault() ??
                   new ISRGlobalSettings();
        }

        /// <summary>
        /// Updates ISR global settings record
        /// </summary>
        /// <param name="settings">Payload with latest settings data</param>
        /// <returns> Returns updated settings record </returns>
        /// <exception cref="InvalidOperationException">When there is more than one record in data source</exception>
        public ISRGlobalSettings Update(ISRGlobalSettings settings)
        {
            lock (_dbContext)
            {
                var existingSettings = Get();

                existingSettings.FulfillFrom(settings);
                _dbContext.Update(existingSettings);

                return existingSettings;
            }
        }
    }
}
