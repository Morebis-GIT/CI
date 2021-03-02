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
    public class InventoryTypeRepository : IInventoryTypeRepository
    {
        private const string IsSystemInventoryType = "Y";

        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public InventoryTypeRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<InventoryType> GetAll() =>
            _dbContext.Query<Entities.Tenant.InventoryStatuses.InventoryType>()
                .ProjectTo<InventoryType>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<InventoryType> GetSystemInventories() =>
            _dbContext
                .Query<Entities.Tenant.InventoryStatuses.InventoryType>()
                .Where(e => e.System == IsSystemInventoryType)
                .ProjectTo<InventoryType>(_mapper.ConfigurationProvider)
                .ToList();

        public InventoryType Get(int id) =>
            _dbContext.Query<Entities.Tenant.InventoryStatuses.InventoryType>()
                .ProjectTo<InventoryType>(_mapper.ConfigurationProvider)
                .FirstOrDefault(t => t.Id == id);

        public void AddRange(IEnumerable<InventoryType> inventoryTypes) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.InventoryStatuses.InventoryType[]>(inventoryTypes),
                post => post.MapToCollection(inventoryTypes), _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.InventoryStatuses.InventoryType>();
    }
}
