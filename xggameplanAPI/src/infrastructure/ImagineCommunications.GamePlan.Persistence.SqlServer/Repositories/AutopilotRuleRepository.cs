using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutopilotRuleRepository : IAutopilotRuleRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AutopilotRuleRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(AutopilotRule item) => _dbContext.Add(_mapper.Map<Entities.Tenant.AutopilotRule>(item), post => post.MapTo(item), _mapper);

        public void Update(AutopilotRule item)
        {
            var entity = _dbContext.Find<Entities.Tenant.AutopilotRule>(item.Id);
            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.AutopilotRule>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Delete(IEnumerable<int> ids)
        {
            var entities = _dbContext
                .Query<Entities.Tenant.AutopilotRule>()
                .Where(x => ids.Contains(x.Id))
                .ToArray();
            _dbContext.RemoveRange(entities);
        }

        public AutopilotRule Get(int id) => _mapper.Map<AutopilotRule>(_dbContext.Find<Entities.Tenant.AutopilotRule>(id));

        public IEnumerable<AutopilotRule> GetAll()
        {
            return _dbContext
               .Query<Entities.Tenant.AutopilotRule>()
               .ProjectTo<AutopilotRule>(_mapper.ConfigurationProvider)
               .ToList();
        }

        public IEnumerable<AutopilotRule> GetByFlexibilityLevelId(int id)
        {
            return _dbContext
               .Query<Entities.Tenant.AutopilotRule>()
               .Where(x => x.FlexibilityLevelId == id) // check
               .ProjectTo<AutopilotRule>(_mapper.ConfigurationProvider)
               .ToList();
        }

        public void SaveChanges() => _dbContext.SaveChanges();

    }
}
