using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Settings
{
    public class InventoryLock : ICloneable
    {
        public bool Locked { get; set; }
        public Guid? ChosenScenarioId { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
