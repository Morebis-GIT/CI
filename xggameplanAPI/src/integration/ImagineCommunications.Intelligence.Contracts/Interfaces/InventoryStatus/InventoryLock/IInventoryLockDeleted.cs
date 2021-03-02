using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock
{
    public interface IInventoryLockDeleted : IEvent
    {
        string SalesArea { get; }
    }
}
