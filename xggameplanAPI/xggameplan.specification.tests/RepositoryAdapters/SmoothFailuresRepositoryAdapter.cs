using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SmoothFailuresRepositoryAdapter
         : RepositoryTestAdapter<SmoothFailure, ISmoothFailureRepository, Guid>
    {
        public SmoothFailuresRepositoryAdapter(
            IScenarioDbContext dbContext,
            ISmoothFailureRepository repository
        ) : base(dbContext, repository) { }

        protected override IEnumerable<SmoothFailure> AddRange(params SmoothFailure[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override SmoothFailure GetById(Guid id) =>
            throw new NotImplementedException();

        [RepositoryMethod]
        protected CallMethodResult GetByRunId(Guid runId) =>
            CallMethodResult.Create(Repository.GetByRunId(runId));

        [RepositoryMethod]
        protected CallMethodResult RemoveByRunId(Guid runId)
        {
            Repository.RemoveByRunId(runId);
            Repository.SaveChanges();

            return CallMethodResult.CreateHandled();
        }

        protected override SmoothFailure Add(SmoothFailure model) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override IEnumerable<SmoothFailure> GetAll() =>
            throw new NotImplementedException();

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override SmoothFailure Update(SmoothFailure model) =>
            throw new NotImplementedException();

        protected override void Delete(Guid id) =>
            throw new NotImplementedException();
    }
}
