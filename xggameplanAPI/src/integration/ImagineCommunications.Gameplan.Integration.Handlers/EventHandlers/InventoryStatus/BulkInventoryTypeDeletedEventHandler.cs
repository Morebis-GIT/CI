using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus
{
    public class BulkInventoryTypeDeletedEventHandler : IEventHandler<IBulkInventoryTypeDeleted>
    {
        private readonly IInventoryTypeRepository _inventoryTypeRepository;

        public BulkInventoryTypeDeletedEventHandler(IInventoryTypeRepository inventoryTypeRepository)
        {
            _inventoryTypeRepository = inventoryTypeRepository;
        }
        public void Handle(IBulkInventoryTypeDeleted command)
        {
            _inventoryTypeRepository.Truncate();
            _inventoryTypeRepository.SaveChanges();
        }
    }
}
