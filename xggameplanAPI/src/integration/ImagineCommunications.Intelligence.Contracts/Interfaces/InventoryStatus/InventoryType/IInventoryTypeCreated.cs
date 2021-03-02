using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType
{
    public interface IInventoryTypeCreated : IEvent
    {
        string InventoryCode { get; }
        string Description { get; }
        string System { get; }
        IEnumerable<int> LockTypes { get; } 
    }
}
