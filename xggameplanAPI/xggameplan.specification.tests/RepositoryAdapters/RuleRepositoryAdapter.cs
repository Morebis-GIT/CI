using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RuleRepositoryAdapter : RepositoryTestAdapter<Rule, IRuleRepository, int>
    {
        public RuleRepositoryAdapter(IScenarioDbContext dbContext, IRuleRepository repository) : base(dbContext, repository)
        {
        }

        protected override Rule Add(Rule model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Rule> AddRange(params Rule[] models) =>
            throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<Rule> GetAll() => Repository.GetAll();

        protected override Rule GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override Rule Update(Rule model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
