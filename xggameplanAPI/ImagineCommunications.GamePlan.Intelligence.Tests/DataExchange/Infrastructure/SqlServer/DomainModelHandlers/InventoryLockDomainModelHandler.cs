using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using InventoryLockEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses.InventoryLock;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class InventoryLockDomainModelHandler : SimpleDomainModelMappingHandler<InventoryLockEntity, InventoryLock>
    {
        private readonly IInventoryLockRepository _repository;

        public InventoryLockDomainModelHandler(IInventoryLockRepository repository, ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _repository = repository;
        }

        public override InventoryLock Add(InventoryLock model)
        {
            _repository.AddRange(new[] {model});
            return model;
        }

        public override void AddRange(params InventoryLock[] models)
        {
            _repository.AddRange(models);
        }

        public override IEnumerable<InventoryLock> GetAll() => _repository.GetAll();
    }
}
