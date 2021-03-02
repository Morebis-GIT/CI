using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class LockTypeRepositoryAdapter : RepositoryTestAdapter<InventoryLockType, ILockTypeRepository, int>
    {
        public LockTypeRepositoryAdapter(IScenarioDbContext dbContext, ILockTypeRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override InventoryLockType Add(InventoryLockType model) => throw new NotImplementedException();

        protected override IEnumerable<InventoryLockType> AddRange(params InventoryLockType[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override InventoryLockType Update(InventoryLockType model) => throw new NotImplementedException();

        protected override InventoryLockType GetById(int id) => Repository.Get(id);

        protected override IEnumerable<InventoryLockType> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}
