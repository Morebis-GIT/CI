using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ScenarioCampaignResultRepositoryAdapter : RepositoryTestAdapter<ScenarioCampaignResult, IScenarioCampaignResultRepository, Guid>
    {
        public ScenarioCampaignResultRepositoryAdapter(IScenarioDbContext dbContext, IScenarioCampaignResultRepository repository) : base(
            dbContext, repository)
        {
        }

        protected override ScenarioCampaignResult Add(ScenarioCampaignResult model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }

        protected override IEnumerable<ScenarioCampaignResult> AddRange(params ScenarioCampaignResult[] models)
            => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(Guid id)
            => Repository.Delete(id);

        protected override IEnumerable<ScenarioCampaignResult> GetAll()
            => DbContext.GetAll<ScenarioCampaignResult>();

        protected override ScenarioCampaignResult GetById(Guid id)
            => Repository.Get(id);

        protected override void Truncate()
            => throw new NotImplementedException();

        protected override ScenarioCampaignResult Update(ScenarioCampaignResult model)
            => throw new NotImplementedException();
    }
}

