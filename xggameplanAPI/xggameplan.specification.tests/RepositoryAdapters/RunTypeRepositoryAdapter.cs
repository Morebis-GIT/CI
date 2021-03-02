using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RunTypeRepositoryAdapter : RepositoryTestAdapter<RunType, IRunTypeRepository, int>
    {
        public RunTypeRepositoryAdapter(IScenarioDbContext dbContext, IRunTypeRepository repository) : base(
            dbContext, repository)
        {
        }

        protected override RunType Add(RunType model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<RunType> AddRange(params RunType[] models)
        {
            foreach (var model in models)
            {
                Repository.Add(model);
            }
            return models;
        }

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id)
            => Repository.Delete(id);

        protected override IEnumerable<RunType> GetAll()
            => DbContext.GetAll<RunType>();

        protected override RunType GetById(int id)
            => Repository.Get(id);

        protected override void Truncate()
            => throw new NotImplementedException();

        protected override RunType Update(RunType model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
