using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
{
    public class SalesAreaTestCacheAccessor : SalesAreaCacheAccessor
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;

        public SalesAreaTestCacheAccessor(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory) : base(dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override IReadOnlyCollection<SalesArea> GetSalesAreas()
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                return dbContext.Query<SalesArea>().ToArray();
            }
        }
    }
}
