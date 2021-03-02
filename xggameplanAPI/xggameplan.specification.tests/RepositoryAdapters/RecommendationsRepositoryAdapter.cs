using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RecommendationsRepositoryAdapter : RepositoryTestAdapter<Recommendation, IRecommendationRepository, int>
    {
        public RecommendationsRepositoryAdapter(IScenarioDbContext dbContext, IRecommendationRepository repository) : base(dbContext, repository)
        {
        }

        protected override IEnumerable<Recommendation> AddRange(params Recommendation[] models)
        {
            Repository.Insert(models);
            return models;
        }

        #region Not used methods
        protected override Recommendation Add(Recommendation model) => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override IEnumerable<Recommendation> GetAll() => throw new NotImplementedException();

        protected override Recommendation GetById(int id) => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();

        protected override Recommendation Update(Recommendation model) => throw new NotImplementedException();
        #endregion

        [RepositoryMethod]
        protected CallMethodResult RemoveByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.RemoveByScenarioIdAndProcessors(scenarioId, processors);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult RemoveByScenarioId(Guid scenarioId)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.RemoveByScenarioId(scenarioId);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
