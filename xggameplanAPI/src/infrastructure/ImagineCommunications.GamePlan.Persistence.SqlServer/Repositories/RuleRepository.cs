using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RuleRepository : IRuleRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RuleRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Rule item) =>_dbContext.Add(_mapper.Map<Entities.Tenant.Rule>(item), post => post.MapTo(item), _mapper);

        public void Update(Rule item)
        {
            var entity = _dbContext.Find<Entities.Tenant.Rule>(item.Id);
            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Update(IEnumerable<Rule> items)
        {
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                _mapper.Map<List<Entities.Tenant.Rule>>(items),
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true },
                post => post.TryToUpdate(items),
                _mapper
            );
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.Rule>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public IEnumerable<Rule> FindByRuleTypeId(int ruleTypeId)
        {
            return _mapper.Map<IEnumerable<Rule>>(
              _dbContext
              .Query<Entities.Tenant.Rule>()
              .Where(x => x.RuleTypeId == ruleTypeId)
              .ToList()
            );
        }

        public IEnumerable<Rule> FindByRuleTypeIds(IEnumerable<int> ruleTypeIds)
        {
            return _mapper.Map<IEnumerable<Rule>>(
                _dbContext
                .Query<Entities.Tenant.Rule>()
                .Where(x => ruleTypeIds.Contains(x.RuleTypeId))
                .ToList()
            );
        }

        public Rule Get(int id) => _mapper.Map<Rule>(_dbContext.Find<Entities.Tenant.Rule>(id));

        public IEnumerable<Rule> GetAll()
        {
            return _mapper.Map<IEnumerable<Rule>>(
                _dbContext
                .Query<Entities.Tenant.Rule>()
                .ToList()
            );
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
