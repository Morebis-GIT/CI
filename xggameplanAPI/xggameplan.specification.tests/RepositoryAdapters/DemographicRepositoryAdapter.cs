using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class DemographicRepositoryAdapter : RepositoryTestAdapter<Demographic, IDemographicRepository, int>
    {
        public DemographicRepositoryAdapter(IScenarioDbContext dbContext, IDemographicRepository repository) : base(dbContext, repository)
        {
        }

        protected override Demographic Add(Demographic model)
        {
            Repository.Add(new [] {model});
            return model;
        }

        protected override IEnumerable<Demographic> AddRange(params Demographic[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Demographic Update(Demographic model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Demographic GetById(int id)
        {
            return Repository.GetById(id);
        }

        protected override IEnumerable<Demographic> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            Repository.Truncate();
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }
    }
}
