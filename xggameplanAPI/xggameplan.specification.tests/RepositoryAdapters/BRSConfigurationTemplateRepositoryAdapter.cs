using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BRS;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class BRSConfigurationTemplateRepositoryAdapter : RepositoryTestAdapter<BRSConfigurationTemplate, IBRSConfigurationTemplateRepository, int>
    {
        public BRSConfigurationTemplateRepositoryAdapter(IScenarioDbContext dbContext, IBRSConfigurationTemplateRepository repository) : base(dbContext, repository)
        {
        }

        protected override BRSConfigurationTemplate Add(BRSConfigurationTemplate model)
        {
            Repository.Add(model);
            return model;
        }

        protected override BRSConfigurationTemplate Update(BRSConfigurationTemplate model)
        {
            Repository.Update(model);
            return model;
        }

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<BRSConfigurationTemplate> GetAll() => Repository.GetAll();

        protected override BRSConfigurationTemplate GetById(int id) => Repository.Get(id);

        protected override IEnumerable<BRSConfigurationTemplate> AddRange(params BRSConfigurationTemplate[] models) => throw new NotImplementedException();

        protected override int Count() => Repository.Count();

        protected override void Truncate() => throw new NotImplementedException();

        [RepositoryMethod]
        protected CallMethodResult GetDefault()
        {
            return CallMethodResult.Create(Repository.GetDefault());
        }

        [RepositoryMethod]
        protected CallMethodResult ChangeDefaultConfiguration(int id)
        {
            _ = Repository.ChangeDefaultConfiguration(id);
            return CallMethodResult.CreateHandled();
        }
    }
}
