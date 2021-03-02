using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class InventoryLockRepositoryAdapter : RepositoryTestAdapter<InventoryLock, IInventoryLockRepository, int>
    {
        public InventoryLockRepositoryAdapter(IScenarioDbContext dbContext, IInventoryLockRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override InventoryLock Add(InventoryLock model) => throw new NotImplementedException();

        protected override IEnumerable<InventoryLock> AddRange(params InventoryLock[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override InventoryLock Update(InventoryLock model) => throw new NotImplementedException();

        protected override InventoryLock GetById(int id) => Repository.Get(id);

        protected override IEnumerable<InventoryLock> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();

        [RepositoryMethod]
        protected CallMethodResult DeleteRange(IEnumerable<string> salesAreas)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteRange(salesAreas);
            DbContext.SaveChanges();

            return CallMethodResult.CreateHandled();
        }
    }
}
