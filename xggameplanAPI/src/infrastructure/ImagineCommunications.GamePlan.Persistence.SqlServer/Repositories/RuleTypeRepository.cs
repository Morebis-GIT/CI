using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RuleTypeRepository : IRuleTypeRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RuleTypeRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(RuleType item) => _dbContext.Add(_mapper.Map<Entities.Tenant.RuleType>(item),
            post => post.MapTo(item), _mapper);

        public void Update(RuleType item)
        {
            var entity = _dbContext.Find<Entities.Tenant.RuleType>(item.Id);
            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Update(IEnumerable<RuleType> items)
        {
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                _mapper.Map<List<Entities.Tenant.RuleType>>(items),
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true },
                post => post.TryToUpdate(items),
                _mapper
            );
        }


        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.RuleType>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public RuleType Get(int id) => _mapper.Map<RuleType>(_dbContext.Find<Entities.Tenant.RuleType>(id));

        public IEnumerable<RuleType> GetAll(bool onlyAllowedAutopilot = false)
        {
            return _dbContext
                .Query<Entities.Tenant.RuleType>()
                .Where(x => !onlyAllowedAutopilot || x.AllowedForAutopilot)
                .ProjectTo<RuleType>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
