using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DomainModelContext;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    public class RavenScenarioDbContext : RavenDomainModelContext, IScenarioDbContext
    {
        private readonly IRavenTestDbContext _dbContext;

        public RavenScenarioDbContext(IRavenTestDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void WaitForIndexesAfterSaveChanges()
        {
            _dbContext.Specific.WaitForIndexesAfterSaveChanges();
        }

        public void WaitForIndexesToBeFresh()
        {
            _dbContext.Specific.WaitForIndexes();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
            _dbContext.SaveChangesAsync().Wait();

            _dbContext.Specific.WaitForIndexes();
        }

        public void Cleanup()
        {
            _dbContext.Specific.DeleteByIndex("Raven/DocumentsByEntityName");
            _dbContext.Specific.WaitForIndexes();
        }
    }
}
