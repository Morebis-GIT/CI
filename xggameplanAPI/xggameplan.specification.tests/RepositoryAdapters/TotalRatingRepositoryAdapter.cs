using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class TotalRatingRepositoryAdapter : RepositoryTestAdapter<TotalRating, ITotalRatingRepository, int>
    {
        public TotalRatingRepositoryAdapter(IScenarioDbContext dbContext, ITotalRatingRepository repository) : base(
            dbContext, repository)
        {
        }

        protected override TotalRating Add(TotalRating model) => throw new NotImplementedException();

        protected override IEnumerable<TotalRating> AddRange(params TotalRating[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override TotalRating Update(TotalRating model) => throw new NotImplementedException();

        protected override TotalRating GetById(int id) => Repository.Get(id);

        protected override IEnumerable<TotalRating> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();

        [RepositoryMethod]
        protected CallMethodResult DeleteRange(IEnumerable<int> ids)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteRange(ids);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
