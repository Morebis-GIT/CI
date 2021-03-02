using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SpotRepositoryAdapter : RepositoryTestAdapter<Spot, ISpotRepository, Guid>
    {
        public SpotRepositoryAdapter(IScenarioDbContext dbContext, ISpotRepository repository) : base(dbContext, repository)
        {
        }

        protected override Spot Add(Spot model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Spot> AddRange(params Spot[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        protected override void Delete(Guid id)
        {
            Repository.Remove(id);
        }

        protected override IEnumerable<Spot> GetAll()
        {
            return Repository.GetAll();
        }

        protected override Spot GetById(Guid id)
        {
            return Repository.Find(id);
        }

        protected override void Truncate()
        {
            Repository.TruncateAsync().Wait();
        }

        protected override Spot Update(Spot model)
        {
            Repository.Update(model);
            return model;
        }

        [RepositoryMethod]
        protected CallMethodResult Delete(IList<Guid> ids)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Delete(ids);
            DbContext.SaveChanges();

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult InsertOrReplace(string externalCampaignNumber, string salesArea, string externalBreakNo, string multipartSpot, Guid id, string externalSpotRef)
        {
            var spot = new Spot
            {
                ExternalCampaignNumber = externalCampaignNumber,
                SalesArea = salesArea,
                ExternalBreakNo = externalBreakNo,
                MultipartSpot = multipartSpot,
                Uid = id,
                ExternalSpotRef = externalSpotRef
            };

            Repository.InsertOrReplace(new[] { spot });
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
