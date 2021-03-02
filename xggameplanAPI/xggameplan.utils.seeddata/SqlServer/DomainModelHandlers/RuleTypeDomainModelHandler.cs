using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RuleTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RuleType;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RuleTypeDomainModelHandler : IDomainModelHandler<RuleType>
    {
        private readonly IRuleTypeRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public RuleTypeDomainModelHandler(IRuleTypeRepository ruleTypeRepository, ISqlServerDbContext dbContext)
        {
            _repository = ruleTypeRepository ?? throw new ArgumentNullException(nameof(ruleTypeRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public RuleType Add(RuleType model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params RuleType[] models)
        {
            foreach(var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<RuleTypeEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<RuleTypeEntity>();

        public IEnumerable<RuleType> GetAll() => _repository.GetAll();
    }
}
