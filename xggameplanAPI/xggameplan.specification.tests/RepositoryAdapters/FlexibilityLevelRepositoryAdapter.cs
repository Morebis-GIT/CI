using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class FlexibilityLevelRepositoryAdapter : RepositoryTestAdapter<FlexibilityLevel, IFlexibilityLevelRepository, int>
    {
        public FlexibilityLevelRepositoryAdapter(IScenarioDbContext dbContext, IFlexibilityLevelRepository repository) : base(dbContext, repository)
        {
        }

        protected override FlexibilityLevel Add(FlexibilityLevel model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<FlexibilityLevel> AddRange(params FlexibilityLevel[] models) =>
            throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<FlexibilityLevel> GetAll() => Repository.GetAll();

        protected override FlexibilityLevel GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override FlexibilityLevel Update(FlexibilityLevel model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
