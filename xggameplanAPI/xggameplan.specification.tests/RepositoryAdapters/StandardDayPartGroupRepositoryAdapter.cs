using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class StandardDayPartGroupRepositoryAdapter : RepositoryTestAdapter<StandardDayPartGroup, IStandardDayPartGroupRepository, int>
    {
        public StandardDayPartGroupRepositoryAdapter(IScenarioDbContext dbContext, IStandardDayPartGroupRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override StandardDayPartGroup Add(StandardDayPartGroup model) => throw new NotImplementedException();

        protected override IEnumerable<StandardDayPartGroup> AddRange(params StandardDayPartGroup[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override StandardDayPartGroup Update(StandardDayPartGroup model) => throw new NotImplementedException();

        protected override StandardDayPartGroup GetById(int id) => Repository.Get(id);

        protected override IEnumerable<StandardDayPartGroup> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}

