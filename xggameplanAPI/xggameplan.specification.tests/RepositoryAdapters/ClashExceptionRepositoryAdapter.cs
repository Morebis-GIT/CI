using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ClashExceptionRepositoryAdapter : RepositoryTestAdapter<ClashException, IClashExceptionRepository, int>
    {
        public ClashExceptionRepositoryAdapter(IScenarioDbContext dbContext, IClashExceptionRepository repository) : base(dbContext, repository)
        {
        }

        protected override ClashException Add(ClashException model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<ClashException> AddRange(params ClashException[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        protected override void Delete(int id)
        {
            Repository.Remove(id);
        }

        protected override IEnumerable<ClashException> GetAll()
        {
            return Repository.GetAll();
        }

        protected override ClashException GetById(int id)
        {
            return Repository.Find(id);
        }

        protected override void Truncate()
        {
            Repository.Truncate();
        }

        protected override ClashException Update(ClashException model)
        {
            throw new NotImplementedException();
        }

        [RepositoryMethod]
        protected CallMethodResult Search(DateTime? startDate, DateTime? endDate)
        {
            var queryModel = new ClashExceptionSearchQueryModel
            {
                StartDate = startDate ?? default,
                EndDate = endDate ?? default
            };
            var res = Repository.Search(queryModel);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = null;

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteRangeByExternalRefs(externalRefs);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
