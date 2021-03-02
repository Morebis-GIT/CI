using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AWSInstanceConfigurationRepositoryAdapter :
        RepositoryTestAdapter<AWSInstanceConfiguration, IAWSInstanceConfigurationRepository, int>
    {
        public AWSInstanceConfigurationRepositoryAdapter(
            IScenarioDbContext dbContext,
            IAWSInstanceConfigurationRepository repository
            ) : base(dbContext, repository) { }

        protected override AWSInstanceConfiguration Add(AWSInstanceConfiguration model) =>
            throw new NotImplementedException();

        protected override IEnumerable<AWSInstanceConfiguration> AddRange(params AWSInstanceConfiguration[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(int id) =>
            throw new NotImplementedException();

        protected override IEnumerable<AWSInstanceConfiguration> GetAll() =>
            Repository.GetAll();

        protected override AWSInstanceConfiguration GetById(int id) =>
            Repository.Get(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override AWSInstanceConfiguration Update(AWSInstanceConfiguration model) =>
            throw new NotImplementedException();
    }
}
