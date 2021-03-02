using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Dsl;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class PassRepositoryAdapter : RepositoryTestAdapter<Pass, IPassRepository, int>
    {
        public PassRepositoryAdapter(IScenarioDbContext dbContext, IPassRepository repository) : base(dbContext, repository)
        {
        }

        protected override IPostprocessComposer<Pass> GetAutoModelComposer() =>
            base.GetAutoModelComposer().With(x => x.RatingPoints, () => new List<RatingPoint>());

        protected override Pass Add(Pass model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Pass> AddRange(params Pass[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Pass Update(Pass model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Pass GetById(int id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Pass> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        [RepositoryMethod]
        protected CallMethodResult Remove(IEnumerable<int> ids)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Remove(ids);
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

        [RepositoryMethod]
        protected CallMethodResult Search(
            string name,
            bool? isLibraried,
            StringMatchHowManyWordsToMatch howManyWordsToMatch,
            StringMatchWordOrders wordOrder,
            StringMatchWordComparisons wordComparison,
            bool caseSensitive)
        {
            var queryModel = new PassSearchQueryModel
            {
                Name = name,
                IsLibraried = isLibraried
            };

            var freeTextMatchRules = new StringMatchRules(howManyWordsToMatch, wordOrder, wordComparison, caseSensitive,
                new Char[] { ' ', ',' }, null);
            var res = Repository.Search(queryModel, freeTextMatchRules);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items?.Cast<object>().ToList();
            TestContext.LastSingleResult = TestContext.LastOperationCount == 1 ? res.Items.First() : null;

            return CallMethodResult.CreateHandled();
        }
    }
}
