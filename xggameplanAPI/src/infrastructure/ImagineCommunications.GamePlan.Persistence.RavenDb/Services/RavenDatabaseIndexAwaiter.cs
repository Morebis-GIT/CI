using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    public class RavenDatabaseIndexAwaiter : IDatabaseIndexAwaiter
    {
        private readonly IRavenDbContext _dbContext;

        public RavenDatabaseIndexAwaiter(IRavenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task WaitForIndexAsync(string indexName)
        {
            await _dbContext.Specific.WaitForIndexAsync(indexName).ConfigureAwait(false);
        }

        public async Task WaitForIndexesAsync()
        {
            await _dbContext.Specific.WaitForIndexesAsync().ConfigureAwait(false);
        }
    }
}
