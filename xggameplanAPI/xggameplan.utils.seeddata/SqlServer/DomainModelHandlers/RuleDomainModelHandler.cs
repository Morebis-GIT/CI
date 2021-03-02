using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Rule;


namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RuleDomainModelHandler : IDomainModelHandler<Rule>
    {
        private readonly IRuleRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public RuleDomainModelHandler(IRuleRepository ruleRepository, ISqlServerDbContext dbContext)
        {
            _repository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Rule Add(Rule model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params Rule[] models)
        {
            foreach(var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<RuleEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<RuleEntity>();

        public IEnumerable<Rule> GetAll() => _repository.GetAll();
    }
}
