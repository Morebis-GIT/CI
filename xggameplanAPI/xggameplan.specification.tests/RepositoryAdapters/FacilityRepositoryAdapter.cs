using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class FacilityRepositoryAdapter : RepositoryTestAdapter<Facility, IFacilityRepository, int>
    {
        public FacilityRepositoryAdapter(IScenarioDbContext dbContext, IFacilityRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override Facility Add(Facility model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Facility> AddRange(params Facility[] models) => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<Facility> GetAll() => Repository.GetAll();

        protected override Facility GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override Facility Update(Facility model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
