using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class LockTypeRepository : ILockTypeRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public LockTypeRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<InventoryLockType> GetAll() =>
            _dbContext.Query<Entities.Tenant.InventoryStatuses.InventoryLockType>()
                .ProjectTo<InventoryLockType>(_mapper.ConfigurationProvider)
                .ToList();

        public InventoryLockType Get(int id) => _mapper.Map<InventoryLockType>(_dbContext.Find<Entities.Tenant.InventoryStatuses.InventoryLockType>(id));

        public void AddRange(IEnumerable<InventoryLockType> lockTypes) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.InventoryStatuses.InventoryLockType[]>(lockTypes),
                post => post.MapToCollection(lockTypes), _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.InventoryStatuses.InventoryLockType>();
    }
}
