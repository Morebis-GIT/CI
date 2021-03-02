using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SalesAreaDemographicRepositoryAdapter : RepositoryTestAdapter<SalesAreaDemographic, ISalesAreaDemographicRepository, int>
    {
        public SalesAreaDemographicRepositoryAdapter(IScenarioDbContext dbContext, ISalesAreaDemographicRepository repository) : base(dbContext, repository)
        {
        }

        protected override IEnumerable<SalesAreaDemographic> AddRange(params SalesAreaDemographic[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override IEnumerable<SalesAreaDemographic> GetAll() => Repository.GetAll();

        [RepositoryMethod]
        protected CallMethodResult DeleteBySalesAreaName(string name)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteBySalesAreaName(name);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }

        protected override SalesAreaDemographic Add(SalesAreaDemographic model) => throw new NotImplementedException();
        protected override SalesAreaDemographic Update(SalesAreaDemographic model) => throw new NotImplementedException();
        protected override void Delete(int id) => throw new NotImplementedException();
        protected override SalesAreaDemographic GetById(int id) => throw new NotImplementedException();
        protected override int Count() => throw new NotImplementedException();
        protected override void Truncate() => throw new NotImplementedException();
    }
}
