using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType
{
    public interface ILockTypeCreated : IEvent
    {
        int LockType { get; }
        string Name { get; }
    }
}
