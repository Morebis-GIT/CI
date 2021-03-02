using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Cache;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Caches
{
    public class ProgrammeCategoryCache : SqlServerEntityCache<string, ProgrammeCategory>
    {
        public ProgrammeCategoryCache(ISqlServerDbContext dbContext, bool preloadData = true,
            bool trackingChanges = true)
            : base(dbContext, x => x.Name, preloadData, trackingChanges)
        {
        }

        protected override string AdjustKeyValue(string key) => key?.ToLowerInvariant();
    }
}
