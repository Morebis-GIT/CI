using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ScenarioCampaignFailureRepositoryAdapter : RepositoryTestAdapter<ScenarioCampaignFailure, IScenarioCampaignFailureRepository, int>
    {
        public ScenarioCampaignFailureRepositoryAdapter(IScenarioDbContext dbContext, IScenarioCampaignFailureRepository repository) : base(dbContext, repository)
        {
        }

        protected override ScenarioCampaignFailure Add(ScenarioCampaignFailure model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<ScenarioCampaignFailure> AddRange(params ScenarioCampaignFailure[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override IEnumerable<ScenarioCampaignFailure> GetAll()
        {
            return Repository.GetAll();
        }

        protected override ScenarioCampaignFailure GetById(int id)
        {
            return Repository.Get(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override ScenarioCampaignFailure Update(ScenarioCampaignFailure model)
        {
            throw new NotImplementedException();
        }
    }
}
