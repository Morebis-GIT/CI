using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryLock
{
    public class InventoryLockDeleted : IInventoryLockDeleted
    {
        public string SalesArea { get; }

        public InventoryLockDeleted(string salesArea) => SalesArea = salesArea;
    }
}
