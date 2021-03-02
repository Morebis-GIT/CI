using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class StandardDayPartRepositoryAdapter : RepositoryTestAdapter<StandardDayPart, IStandardDayPartRepository, int>
    {
        public StandardDayPartRepositoryAdapter(IScenarioDbContext dbContext, IStandardDayPartRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override StandardDayPart Add(StandardDayPart model) => throw new NotImplementedException();

        protected override IEnumerable<StandardDayPart> AddRange(params StandardDayPart[] models)
        {
           Repository.AddRange(models);
           return models;
        }

        protected override StandardDayPart Update(StandardDayPart model) => throw new NotImplementedException();

        protected override StandardDayPart GetById(int id) => Repository.Get(id);

        protected override IEnumerable<StandardDayPart> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}
