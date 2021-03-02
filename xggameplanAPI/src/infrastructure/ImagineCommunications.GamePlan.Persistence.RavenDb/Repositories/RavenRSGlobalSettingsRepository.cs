using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRSGlobalSettingsRepository : IRSGlobalSettingsRepository
    {
        private readonly IRavenTenantDbContext _dbContext;

        public RavenRSGlobalSettingsRepository(IRavenTenantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns RS global settings
        /// </summary>
        /// <returns>
        /// Returns RS global settings record typeof <see cref="RSGlobalSettings"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">When there is more than one record in data source</exception>
        public RSGlobalSettings Get()
        {
            return _dbContext.Query<RSGlobalSettings>().SingleOrDefault() ??
                   new RSGlobalSettings();
        }

        /// <summary>
        /// Updates RS global settings record
        /// </summary>
        /// <param name="settings">Payload with latest settings data</param>
        /// <returns> Returns updated settings record </returns>
        /// <exception cref="InvalidOperationException">When there is more than one record in data source</exception>
        public RSGlobalSettings Update(RSGlobalSettings settings)
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
