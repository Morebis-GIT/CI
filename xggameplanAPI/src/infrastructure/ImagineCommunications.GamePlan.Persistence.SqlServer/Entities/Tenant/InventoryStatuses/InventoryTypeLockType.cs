﻿using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses
{
    public class InventoryTypeLockType : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int InventoryTypeId { get; set; }
        public int LockType { get; set; }
    }
}
