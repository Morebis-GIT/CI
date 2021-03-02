using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses
{
    public class InventoryLock : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid? SalesAreaId { get; set; }
        public string InventoryCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public SalesArea SalesArea { get; set; }
    }
}
