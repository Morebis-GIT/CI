using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.LockType
{
    public class LockTypeCreated : ILockTypeCreated
    {
        public int LockType { get; set; }
        public string Name { get; set; }

        public LockTypeCreated(int lockType, string name)
        {
            LockType = lockType;
            Name = name;
        }
    }
}
