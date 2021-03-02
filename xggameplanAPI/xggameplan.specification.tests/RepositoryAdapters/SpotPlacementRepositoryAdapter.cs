using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SpotPlacementRepositoryAdapter : RepositoryTestAdapter<SpotPlacement, ISpotPlacementRepository, int>
    {
        public SpotPlacementRepositoryAdapter(IScenarioDbContext dbContext, ISpotPlacementRepository repository) :
            base(dbContext, repository)
        {
        }

        protected override SpotPlacement Add(SpotPlacement model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<SpotPlacement> AddRange(params SpotPlacement[] models)
        {
            Repository.Insert(models);
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override IEnumerable<SpotPlacement> GetAll()
        {
            throw new NotImplementedException();
        }

        protected override SpotPlacement GetById(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override SpotPlacement Update(SpotPlacement model)
        {
            Repository.Update(model);
            return model;
        }

        [RepositoryMethod]
        protected CallMethodResult DeleteByExternalSpotRef(string externalSpotRef)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Delete(externalSpotRef);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult DeleteBefore(DateTime modifiedTime)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteBefore(modifiedTime);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
