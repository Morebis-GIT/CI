using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class UpdateDetailsRepositoryAdapter
        : RepositoryTestAdapter<UpdateDetails, IUpdateDetailsRepository, Guid>
    {
        public UpdateDetailsRepositoryAdapter(
            IScenarioDbContext dbContext,
            IUpdateDetailsRepository repository
            ) : base(dbContext, repository) { }

        protected override UpdateDetails Add(UpdateDetails model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<UpdateDetails> AddRange(params UpdateDetails[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(Guid id) =>
            Repository.Remove(id);

        protected override IEnumerable<UpdateDetails> GetAll() =>
            Repository.GetAll();

        protected override UpdateDetails GetById(Guid id) =>
            Repository.Find(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override UpdateDetails Update(UpdateDetails model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
