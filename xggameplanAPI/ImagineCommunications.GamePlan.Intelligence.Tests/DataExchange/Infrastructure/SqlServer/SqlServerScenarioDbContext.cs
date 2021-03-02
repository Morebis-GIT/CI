using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
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
