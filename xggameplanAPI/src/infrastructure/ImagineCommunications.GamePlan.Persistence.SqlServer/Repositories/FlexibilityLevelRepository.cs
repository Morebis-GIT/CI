using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class FlexibilityLevelRepository : IFlexibilityLevelRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public FlexibilityLevelRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(FlexibilityLevel item) => _dbContext.Add(_mapper.Map<Entities.Tenant.FlexibilityLevel>(item),
            post => post.MapTo(item), _mapper);

        public void Update(FlexibilityLevel item)
        {
            var entity = _dbContext.Find<Entities.Tenant.FlexibilityLevel>(item.Id);
            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.FlexibilityLevel>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public FlexibilityLevel Get(int id) => _mapper.Map<FlexibilityLevel>(_dbContext.Find<Entities.Tenant.FlexibilityLevel>(id));

        public IEnumerable<FlexibilityLevel> GetAll()
        {
            return _dbContext
                .Query<Entities.Tenant.FlexibilityLevel>()
                .ProjectTo<FlexibilityLevel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
