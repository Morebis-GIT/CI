using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Cache;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Caches
{
    public class ProgrammeDictionaryCache : SqlServerEntityCache<string, ProgrammeDictionary>
    {
        public ProgrammeDictionaryCache(ISqlServerDbContext dbContext, bool preloadData = true, bool trackingChanges = true)
            : base(dbContext, x => x.ExternalReference, preloadData, trackingChanges)
        {
        }

        protected override IQueryable<ProgrammeDictionary> ExtendEntityQuery(
            IQueryable<ProgrammeDictionary> entityQuery)
        {
            entityQuery = entityQuery.Include(x => x.ProgrammeEpisodes);

            if (!TrackingChanges)
            {
                entityQuery = entityQuery.AsNoTracking();
            }

            return entityQuery;
        }
    }
}
