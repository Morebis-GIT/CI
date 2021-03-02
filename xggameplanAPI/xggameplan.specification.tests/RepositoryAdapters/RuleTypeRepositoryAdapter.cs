using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RuleTypeRepositoryAdapter : RepositoryTestAdapter<RuleType, IRuleTypeRepository, int>
    {
        public RuleTypeRepositoryAdapter(IScenarioDbContext dbContext, IRuleTypeRepository repository) : base(dbContext, repository)
        {
        }

        protected override RuleType Add(RuleType model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<RuleType> AddRange(params RuleType[] models) =>
            throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<RuleType> GetAll() => Repository.GetAll();

        protected override RuleType GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override RuleType Update(RuleType model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
