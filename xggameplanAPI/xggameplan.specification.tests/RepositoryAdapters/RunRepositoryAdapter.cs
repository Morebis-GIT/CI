using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Queries;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RunRepositoryAdapter : RepositoryTestAdapter<Run, IRunRepository, Guid>
    {
        public RunRepositoryAdapter(IScenarioDbContext dbContext, IRunRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override Run Add(Run model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Run> AddRange(params Run[] models) =>
            throw new NotImplementedException();

        protected override Run Update(Run model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Run GetById(Guid id) =>
            Repository.Find(id);

        protected override IEnumerable<Run> GetAll() =>
            Repository.GetAll();

        protected override void Delete(Guid id) =>
            Repository.Remove(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        [RepositoryMethod]
        protected CallMethodResult Search(
            string description,
            DateTime? executedStartDate,
            DateTime? executedEndDate,
            List<RunStatus> status,
            StringMatchHowManyWordsToMatch howManyWordsToMatch,
            StringMatchWordOrders wordOrder,
            StringMatchWordComparisons wordComparison,
            bool caseSensitive,
            string charactersToIgnore)
        {
            var queryModel = new RunSearchQueryModel
            {
                Description = description,
                ExecutedStartDate = executedStartDate,
                ExecutedEndDate = executedEndDate,
                Status = status
            };

            var stringMatchRules = new StringMatchRules(howManyWordsToMatch, wordOrder, wordComparison, caseSensitive,
                new char[] {' '}, charactersToIgnore.ToCharArray());

            var res = Repository.Search(queryModel, stringMatchRules);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = null;

            return CallMethodResult.CreateHandled();
        }
    }
}
