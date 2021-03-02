using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus
{
    public class BulkInventoryLockCreatedEventHandler : IEventHandler<IBulkInventoryLockCreated>
    {
        private readonly IMapper _mapper;
        private readonly IInventoryLockRepository _inventoryLockRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;

        public BulkInventoryLockCreatedEventHandler(
            IInventoryLockRepository inventoryLockRepository,
            ISalesAreaRepository salesAreaRepository,
            IMapper mapper)
        {
            _inventoryLockRepository = inventoryLockRepository;
            _salesAreaRepository = salesAreaRepository;
            _mapper = mapper;
        }

        public void Handle(IBulkInventoryLockCreated command)
        {
            var inventoryLocks = _mapper.Map<List<InventoryLock>>(command.Data.ToList());
            _salesAreaRepository.ValidateSalesArea(inventoryLocks.Select(c=>c.SalesArea).ToList());

            _inventoryLockRepository.AddRange(inventoryLocks);
            _inventoryLockRepository.SaveChanges();
        }
    }
}
