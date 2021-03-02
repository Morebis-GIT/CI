using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ScenarioResultsRepositoryAdapter : RepositoryTestAdapter<ScenarioResult, IScenarioResultRepository, Guid>
    {
        public ScenarioResultsRepositoryAdapter(IScenarioDbContext dbContext, IScenarioResultRepository repository) : base(
            dbContext, repository)
        {
        }

        protected override ScenarioResult Add(ScenarioResult model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<ScenarioResult> AddRange(params ScenarioResult[] models)
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Guid id)
        {
            Repository.Remove(id);
        }

        protected override IEnumerable<ScenarioResult> GetAll()
        {
            return Repository.GetAll();
        }

        protected override ScenarioResult GetById(Guid id)
        {
            return Repository.Find(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override ScenarioResult Update(ScenarioResult model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
