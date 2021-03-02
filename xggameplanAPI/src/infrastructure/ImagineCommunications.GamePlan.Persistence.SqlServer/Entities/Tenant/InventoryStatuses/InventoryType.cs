using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses
{
    public class InventoryType : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string InventoryCode { get; set; }
        public string Description { get; set; }
        public string System { get; set; }
        public List<InventoryTypeLockType> LockTypes { get; set; } = new List<InventoryTypeLockType>();
    }
}
