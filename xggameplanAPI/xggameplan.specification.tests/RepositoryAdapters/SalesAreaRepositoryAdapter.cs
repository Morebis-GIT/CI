using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SalesAreaRepositoryAdapter : RepositoryTestAdapter<SalesArea, ISalesAreaRepository, Guid>
    {
        public SalesAreaRepositoryAdapter(IScenarioDbContext dbContext, ISalesAreaRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override SalesArea Add(SalesArea model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<SalesArea> AddRange(params SalesArea[] models)
        {
            Repository.Update(models.ToList());
            return models;
        }

        protected override SalesArea Update(SalesArea model)
        {
            Repository.Update(model);
            return model;
        }

        protected override SalesArea GetById(Guid id)
        {
            return Repository.Find(id);
        }

        protected override IEnumerable<SalesArea> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(Guid id)
        {
            Repository.Remove(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }
    }
}
