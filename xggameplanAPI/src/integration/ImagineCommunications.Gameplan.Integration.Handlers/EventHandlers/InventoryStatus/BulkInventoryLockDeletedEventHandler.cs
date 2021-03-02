using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus
{
    public class BulkInventoryLockDeletedEventHandler : IEventHandler<IBulkInventoryLockDeleted>
    {
        private readonly IInventoryLockRepository _inventoryLockRepository;

        public BulkInventoryLockDeletedEventHandler(IInventoryLockRepository inventoryLockRepository)
        {
            _inventoryLockRepository = inventoryLockRepository;
        }

        public void Handle(IBulkInventoryLockDeleted command)
        {
            var salesAreas = command.Data.Select(x => x.SalesArea).ToList();
            if (salesAreas.Any())
            {
                _inventoryLockRepository.DeleteRange(salesAreas);
                _inventoryLockRepository.SaveChanges();
            }
        }
    }
}
