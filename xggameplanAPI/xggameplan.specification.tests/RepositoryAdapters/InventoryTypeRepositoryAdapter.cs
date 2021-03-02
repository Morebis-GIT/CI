using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class InventoryTypeRepositoryAdapter: RepositoryTestAdapter<InventoryType, IInventoryTypeRepository, int>
    {
        public InventoryTypeRepositoryAdapter(IScenarioDbContext dbContext, IInventoryTypeRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override InventoryType Add(InventoryType model) => throw new NotImplementedException();

        protected override IEnumerable<InventoryType> AddRange(params InventoryType[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override InventoryType Update(InventoryType model) => throw new NotImplementedException();

        protected override InventoryType GetById(int id) => Repository.Get(id);

        protected override IEnumerable<InventoryType> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}
