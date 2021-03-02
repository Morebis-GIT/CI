using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AutoBookInstanceConfigurationRepositoryAdapter :
        RepositoryTestAdapter<AutoBookInstanceConfiguration, IAutoBookInstanceConfigurationRepository, int>
    {
        public AutoBookInstanceConfigurationRepositoryAdapter(
            IScenarioDbContext dbContext,
            IAutoBookInstanceConfigurationRepository repository
            ) : base(dbContext, repository) { }

        protected override AutoBookInstanceConfiguration Add(AutoBookInstanceConfiguration model) =>
            throw new NotImplementedException();

        protected override IEnumerable<AutoBookInstanceConfiguration> AddRange(params AutoBookInstanceConfiguration[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(int id) =>
            throw new NotImplementedException();

        protected override IEnumerable<AutoBookInstanceConfiguration> GetAll() =>
            Repository.GetAll();

        protected override AutoBookInstanceConfiguration GetById(int id) =>
            Repository.Get(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override AutoBookInstanceConfiguration Update(AutoBookInstanceConfiguration model) =>
            throw new NotImplementedException();
    }
}
