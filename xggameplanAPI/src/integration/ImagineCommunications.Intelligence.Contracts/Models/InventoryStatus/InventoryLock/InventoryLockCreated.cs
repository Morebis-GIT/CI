using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryLock
{
    public class InventoryLockCreated : IInventoryLockCreated
    {
        public string SalesArea { get; }
        public string InventoryCode { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public InventoryLockCreated(string salesArea, string inventoryCode, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime)
        {
            SalesArea = salesArea;
            InventoryCode = inventoryCode;
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
