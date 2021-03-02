using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ProgrammeRepositoryAdapter : RepositoryTestAdapter<Programme, IProgrammeRepository, Guid>
    {
        public ProgrammeRepositoryAdapter(IScenarioDbContext dbContext, IProgrammeRepository repository) : base(dbContext, repository)
        {
        }

        protected override Programme Add(Programme model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Programme> AddRange(params Programme[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Programme Update(Programme model)
        {
            throw new NotImplementedException();
        }

        protected override Programme GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Programme> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(Guid id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            Repository.TruncateAsync().Wait();
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        [RepositoryMethod]
        protected CallMethodResult Search(string nameOrRef, List<string> salesAreas, DateTime? fromDateInclusive, DateTime? toDateInclusive)
        {
            var searchQuery = new ProgrammeSearchQueryModel
            {
                FromDateInclusive = fromDateInclusive ?? default,
                ToDateInclusive = toDateInclusive ?? default,
                SalesArea = salesAreas,
                NameOrRef = nameOrRef
            };

            var res = Repository.Search(searchQuery);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = TestContext.LastOperationCount == 1 ? res?.Items.First() : null;

            return CallMethodResult.CreateHandled();
        }
    }
}
