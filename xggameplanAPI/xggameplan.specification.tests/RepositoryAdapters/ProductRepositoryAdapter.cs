using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;
using NodaTime;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ProductRepositoryAdapter : RepositoryTestAdapter<Product, IProductRepository, Guid>
    {
        private readonly IClock _clock;

        public ProductRepositoryAdapter(
            IScenarioDbContext dbContext,
            IProductRepository repository,
            IClock clock) : base(dbContext, repository)
        {
            _clock = clock;
        }

        protected override Product Add(Product model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Product> AddRange(params Product[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Product Update(Product model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Product GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Product> GetAll()
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
        protected CallMethodResult SearchAdvertiser(string nameOrRef)
        {
            var queryModel = new AdvertiserSearchQueryModel
            {
                AdvertiserNameorRef = nameOrRef,
            };
            var res = Repository.Search(queryModel, _clock.GetCurrentInstant().ToDateTimeUtc());

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = TestContext.LastOperationCount == 1 ? res.Items.First() : null;

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult Search(
            string externalIdentifier,
            string name,
            string clashCode,
            DateTime? fromDateInclusive,
            DateTime? toDateInclusive,
            string nameOrRef)
        {
            var queryModel = new ProductSearchQueryModel
            {
                Externalidentifier = externalIdentifier,
                Name = name,
                ClashCode = clashCode,
                FromDateInclusive = fromDateInclusive ?? default,
                ToDateInclusive = toDateInclusive ?? default,
                NameOrRef = nameOrRef
            };
            var res = Repository.Search(queryModel, NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = TestContext.LastOperationCount == 1 ? res.Items.First() : null;

            return CallMethodResult.CreateHandled();
        }
    }
}
