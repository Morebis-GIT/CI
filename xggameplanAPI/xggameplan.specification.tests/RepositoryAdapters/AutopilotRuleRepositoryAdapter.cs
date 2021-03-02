using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AutopilotRuleRepositoryAdapter : RepositoryTestAdapter<AutopilotRule, IAutopilotRuleRepository, int>
    {
        public AutopilotRuleRepositoryAdapter(IScenarioDbContext dbContext, IAutopilotRuleRepository repository) : base(dbContext, repository)
        {
        }

        protected override AutopilotRule Add(AutopilotRule model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<AutopilotRule> AddRange(params AutopilotRule[] models) =>
            throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        [RepositoryMethod]
        protected CallMethodResult Delete(List<int> ids)
        {
            DbContext.WaitForIndexesAfterSaveChanges();

            Repository.Delete(ids);
            DbContext.SaveChanges();

            return CallMethodResult.CreateHandled();
        }

        protected override AutopilotRule GetById(int id) => Repository.Get(id);

        protected override IEnumerable<AutopilotRule> GetAll() => Repository.GetAll();

        protected override void Truncate() => throw new NotImplementedException();

        protected override AutopilotRule Update(AutopilotRule model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
