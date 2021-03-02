using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SmoothConfigurationRepositoryAdapter :
        RepositoryTestAdapter<SmoothConfiguration,
            ISmoothConfigurationRepository, int>
    {
        public SmoothConfigurationRepositoryAdapter(
            IScenarioDbContext dbContext,
            ISmoothConfigurationRepository repository
            ) : base(dbContext, repository)
        {
        }

        protected override SmoothConfiguration Add(SmoothConfiguration model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<SmoothConfiguration> AddRange(params SmoothConfiguration[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(int id) =>
            throw new NotImplementedException();

        protected override IEnumerable<SmoothConfiguration> GetAll() =>
            throw new NotImplementedException();

        protected override SmoothConfiguration GetById(int id) =>
            Repository.GetById(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override SmoothConfiguration Update(SmoothConfiguration model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
