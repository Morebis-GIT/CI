using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutopilotRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotRule;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AutopilotRuleDomainModelHandler : IDomainModelHandler<AutopilotRule>
    {
        private readonly IAutopilotRuleRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public AutopilotRuleDomainModelHandler(IAutopilotRuleRepository autopilotRuleRepository, ISqlServerDbContext dbContext)
        {
            _repository = autopilotRuleRepository ?? throw new ArgumentNullException(nameof(autopilotRuleRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public AutopilotRule Add(AutopilotRule model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params AutopilotRule[] models)
        {
            foreach (var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<AutopilotRuleEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<AutopilotRuleEntity>();

        public IEnumerable<AutopilotRule> GetAll() => _repository.GetAll();
    }
}
