using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ScenarioCampaignMetricRepositoryAdapter : RepositoryTestAdapter<ScenarioCampaignMetric, IScenarioCampaignMetricRepository, Guid>
    {
        public ScenarioCampaignMetricRepositoryAdapter(IScenarioDbContext dbContext, IScenarioCampaignMetricRepository repository) : base(
            dbContext, repository)
        {
        }

        protected override ScenarioCampaignMetric Add(ScenarioCampaignMetric model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }

        protected override IEnumerable<ScenarioCampaignMetric> AddRange(params ScenarioCampaignMetric[] models)
            => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(Guid id)
            => Repository.Delete(id);

        protected override IEnumerable<ScenarioCampaignMetric> GetAll()
            => DbContext.GetAll<ScenarioCampaignMetric>();

        protected override ScenarioCampaignMetric GetById(Guid id)
            => Repository.Get(id);

        protected override void Truncate()
            => throw new NotImplementedException();

        protected override ScenarioCampaignMetric Update(ScenarioCampaignMetric model)
            => throw new NotImplementedException();
    }
}
