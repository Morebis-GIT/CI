using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class DeliveryCappingGroupRepository : IDeliveryCappingGroupRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public DeliveryCappingGroupRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(DeliveryCappingGroup item) => _dbContext.Add(_mapper.Map<Entities.Tenant.DeliveryCappingGroup>(item), post => post.MapTo(item), _mapper);

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.DeliveryCappingGroup>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public DeliveryCappingGroup Get(int id) => _mapper.Map<DeliveryCappingGroup>(_dbContext.Find<Entities.Tenant.DeliveryCappingGroup>(id));

        public DeliveryCappingGroup GetByDescription(string description) => _mapper.Map<DeliveryCappingGroup>(_dbContext
            .Query<Entities.Tenant.DeliveryCappingGroup>()
            .FirstOrDefault(x => x.Description == description));

        public IEnumerable<DeliveryCappingGroup> Get(IEnumerable<int> ids) =>
            _dbContext.Query<Entities.Tenant.DeliveryCappingGroup>()
                .Where(x => ids.Contains(x.Id))
                .ProjectTo<DeliveryCappingGroup>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<DeliveryCappingGroup> GetAll() =>
            _dbContext.Query<Entities.Tenant.DeliveryCappingGroup>()
                .ProjectTo<DeliveryCappingGroup>(_mapper.ConfigurationProvider)
                .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Update(DeliveryCappingGroup item)
        {
            var entity = _dbContext.Find<Entities.Tenant.DeliveryCappingGroup>(item.Id);
            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }
    }
}
