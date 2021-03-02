using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses
{
    public class InventoryLockType : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int LockType { get; set; }
        public string Name { get; set; }
    }
}
