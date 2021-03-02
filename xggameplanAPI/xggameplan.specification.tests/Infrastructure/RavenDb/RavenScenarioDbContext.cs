using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DomainModelContext;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
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
            _dbContext.Specific.WaitForIndexesAsync().Wait();
        }

        public void Cleanup()
        {
            _dbContext.Specific.DeleteByIndex("Raven/DocumentsByEntityName");
            _dbContext.Specific.WaitForIndexes();
        }
    }
}
