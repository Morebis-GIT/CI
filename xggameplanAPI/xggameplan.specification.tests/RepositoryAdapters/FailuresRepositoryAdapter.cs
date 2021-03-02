using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class FailuresRepositoryAdapter : RepositoryTestAdapter<Failures, IFailuresRepository, Guid>
    {
        public FailuresRepositoryAdapter(IScenarioDbContext dbContext, IFailuresRepository repository) : base(dbContext, repository)
        {
        }

        protected override Failures Add(Failures model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Failures> AddRange(params Failures[] models)
        {
            throw new NotImplementedException();
        }

        protected override Failures Update(Failures model)
        {
            throw new NotImplementedException();
        }

        protected override Failures GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Failures> GetAll()
        {
            return DbContext.GetAll<Failures>();
        }

        protected override void Delete(Guid id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }
    }
}
