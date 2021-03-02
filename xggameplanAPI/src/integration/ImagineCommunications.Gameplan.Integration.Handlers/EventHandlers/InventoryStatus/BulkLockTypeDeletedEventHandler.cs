using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus
{
    public class BulkLockTypeDeletedEventHandler : IEventHandler<IBulkLockTypeDeleted>
    {
        private readonly ILockTypeRepository _lockTypeRepository;

        public BulkLockTypeDeletedEventHandler(ILockTypeRepository lockTypeRepository)
        {
            _lockTypeRepository = lockTypeRepository;
        }

        public void Handle(IBulkLockTypeDeleted command)
        {
            _lockTypeRepository.Truncate();
            _lockTypeRepository.SaveChanges();
        }
    }
}
