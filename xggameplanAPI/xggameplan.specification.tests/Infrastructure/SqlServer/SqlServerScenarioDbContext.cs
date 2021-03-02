using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerScenarioDbContext : SqlServerDomainModelContext, IScenarioDbContext
    {
        private readonly ISqlServerTestDbContext _dbContext;

        public SqlServerScenarioDbContext(
            IDomainModelHandlerResolver domainModelHandlerResolver,
            ISqlServerTestDbContext dbContext)
            : base(domainModelHandlerResolver)
        {
            _dbContext = dbContext;
        }

        public void WaitForIndexesAfterSaveChanges()
        {
        }

        public void WaitForIndexesToBeFresh()
        {
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Cleanup()
        {
        }
    }
}
