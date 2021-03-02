using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class DeliveryCappingGroupRepositoryAdapter : RepositoryTestAdapter<DeliveryCappingGroup, IDeliveryCappingGroupRepository, int>
    {
        public DeliveryCappingGroupRepositoryAdapter(IScenarioDbContext dbContext, IDeliveryCappingGroupRepository repository) : base(dbContext, repository)
        {
        }

        protected override DeliveryCappingGroup Add(DeliveryCappingGroup model)
        {
            Repository.Add(model);
            return model;
        }

        protected override DeliveryCappingGroup Update(DeliveryCappingGroup model)
        {
            Repository.Update(model);
            return model;
        }

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<DeliveryCappingGroup> GetAll() => Repository.GetAll();

        protected override DeliveryCappingGroup GetById(int id) => Repository.Get(id);

        protected override IEnumerable<DeliveryCappingGroup> AddRange(params DeliveryCappingGroup[] models) => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();
    }
}
