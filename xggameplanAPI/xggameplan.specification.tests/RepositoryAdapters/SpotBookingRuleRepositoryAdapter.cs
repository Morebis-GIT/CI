using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class
        SpotBookingRuleRepositoryAdapter : RepositoryTestAdapter<SpotBookingRule, ISpotBookingRuleRepository, int>
    {
        public SpotBookingRuleRepositoryAdapter(IScenarioDbContext dbContext, ISpotBookingRuleRepository repository) :
            base(dbContext, repository)
        {
        }

        protected override SpotBookingRule Add(SpotBookingRule model) => throw new NotImplementedException();

        protected override IEnumerable<SpotBookingRule> AddRange(params SpotBookingRule[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override SpotBookingRule Update(SpotBookingRule model) => throw new NotImplementedException();

        protected override SpotBookingRule GetById(int id) => Repository.Get(id);

        protected override IEnumerable<SpotBookingRule> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}
