using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock
{
    public interface IInventoryLockCreated : IEvent
    {
        string SalesArea { get; }
        string InventoryCode { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        TimeSpan StartTime { get; }
        TimeSpan EndTime { get; }
    }
}
