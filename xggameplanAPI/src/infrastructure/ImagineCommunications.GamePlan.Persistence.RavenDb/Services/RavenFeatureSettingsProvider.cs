using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using xggameplan.core.FeatureManagement.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    /// <inheritdoc/>
    public class RavenFeatureSettingsProvider : IFeatureSettingsProvider
    {
        private readonly IRavenMasterDbContext _dbContext;

        public RavenFeatureSettingsProvider(IRavenMasterDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<IFeatureSetting> GetForTenant(int tenantId)
        {
            return _dbContext.Query<TenantProductFeature>().Where(x => x.TenantId == tenantId).Take(int.MaxValue).AsEnumerable();
        }
    }
}
