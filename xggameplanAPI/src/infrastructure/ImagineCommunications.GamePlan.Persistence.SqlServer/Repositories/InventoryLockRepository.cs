using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class InventoryLockRepository : IInventoryLockRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByNullableIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public InventoryLockRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByNullableIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public IEnumerable<InventoryLock> GetAll() =>
            _mapper.Map<List<InventoryLock>>(_dbContext.Query<Entities.Tenant.InventoryStatuses.InventoryLock>(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public InventoryLock Get(int id) => _mapper.Map<InventoryLock>(_dbContext.Find<Entities.Tenant.InventoryStatuses.InventoryLock>(id), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void AddRange(IEnumerable<InventoryLock> inventoryLocks) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.InventoryStatuses.InventoryLock[]>(inventoryLocks, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(inventoryLocks, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.InventoryStatuses.InventoryLock>();

        public void DeleteRange(IEnumerable<string> salesAreas)
        {
            var inventoryLocks = _dbContext.Query<Entities.Tenant.InventoryStatuses.InventoryLock>()
                .Where(x => salesAreas.Contains(x.SalesArea.Name))
                .ToArray();

            if (inventoryLocks.Any())
            {
                _dbContext.RemoveRange(inventoryLocks);
            }
        }
    }
}
