using System;

namespace ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects
{
    public class InventoryLock
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public string InventoryCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
