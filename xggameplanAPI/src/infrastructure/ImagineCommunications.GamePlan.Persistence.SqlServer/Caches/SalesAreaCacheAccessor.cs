using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Caches
{
    public class SalesAreaCacheAccessor : ISqlServerSalesAreaByNameCacheAccessor, ISqlServerSalesAreaByIdCacheAccessor, ISqlServerSalesAreaByNullableIdCacheAccessor
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly Lazy<IReadOnlyCollection<SalesArea>> _salesAreaCollection;
        private readonly Lazy<IDictionary<Guid, SalesArea>> _salesAreaByIdCache;
        private readonly Lazy<IDictionary<string, SalesArea>> _salesAreaByNameCache;

        protected virtual IReadOnlyCollection<SalesArea> GetSalesAreas()
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                return dbContext.Query<SalesArea>().AsNoTracking().ToArray();
            }
        }

        protected IReadOnlyCollection<SalesArea> SalesAreaCollection => _salesAreaCollection.Value;

        public SalesAreaCacheAccessor(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _salesAreaCollection = new Lazy<IReadOnlyCollection<SalesArea>>(GetSalesAreas);
            _salesAreaByIdCache =
                new Lazy<IDictionary<Guid, SalesArea>>(() => SalesAreaCollection.ToDictionary(k => k.Id));
            _salesAreaByNameCache =
                new Lazy<IDictionary<string, SalesArea>>(() => SalesAreaCollection.ToDictionary(k => k.Name));
        }

        public SalesArea Get(Guid key)
        {
            _ = _salesAreaByIdCache.Value.TryGetValue(key, out var item);

            return item;
        }

        public SalesArea Get(string key)
        {
            if (key is null)
            {
                return null;
            }

            _ = _salesAreaByNameCache.Value.TryGetValue(key, out var item);
            return item;
        }

        public SalesArea Get(Guid? key) => key is null
            ? null
            : Get(key.Value);
    }
}
