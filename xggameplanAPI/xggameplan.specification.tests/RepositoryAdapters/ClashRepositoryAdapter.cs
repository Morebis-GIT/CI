using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ClashRepositoryAdapter : RepositoryTestAdapter<Clash, IClashRepository, Guid>
    {
        private static readonly List<string> DowPattern = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public ClashRepositoryAdapter(IScenarioDbContext dbContext, IClashRepository repository) : base(dbContext, repository)
        {
            Fixture.Customize<TimeAndDowAPI>(composer => composer
                .With(x => x.DaysOfWeek, () => new List<string> {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"}));
        }

        protected override Clash Add(Clash model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Clash> AddRange(params Clash[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Clash Update(Clash model)
        {
            throw new NotImplementedException();
        }

        protected override Clash GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Clash> GetAll()
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
            return Repository.Count();
        }

        [RepositoryMethod]
        protected CallMethodResult Search(string nameOrRef)
        {
            var queryModel = new ClashSearchQueryModel
            {
                NameOrRef = nameOrRef
            };
            var res = Repository.Search(queryModel);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = TestContext.LastOperationCount == 1 ? res.Items.First() : null;

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult UpdateRange(Guid uid, string parentExternalIdentifier,
            string description, int defaultOffPeakExposureCount)
        {
            var clash = Repository.Get(uid);
            clash.ParentExternalidentifier = parentExternalIdentifier;
            clash.Description = description;
            clash.DefaultOffPeakExposureCount = defaultOffPeakExposureCount;

            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.UpdateRange(new[] {clash});
            DbContext.SaveChanges();

            return CallMethodResult.CreateHandled();
        }
    }
}
