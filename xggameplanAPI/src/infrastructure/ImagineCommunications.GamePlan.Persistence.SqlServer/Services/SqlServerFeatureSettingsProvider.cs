using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.FeatureManagement.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    /// <inheritdoc/>
    public class SqlServerFeatureSettingsProvider : IFeatureSettingsProvider
    {
        private readonly ISqlServerMasterDbContext _dbContext;

        public SqlServerFeatureSettingsProvider(ISqlServerMasterDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<IFeatureSetting> GetForTenant(int tenantId)
        {
            return _dbContext.Query<TenantProductFeature>()
                .Include(x => x.ParentFeatures)
                .Where(f => f.TenantId == tenantId)
                .AsNoTracking()
                .AsEnumerable();
        }
    }
}
