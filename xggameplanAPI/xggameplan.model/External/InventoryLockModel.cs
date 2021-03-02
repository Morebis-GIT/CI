using System;

namespace xggameplan.Model
{
    public class InventoryLockModel
    {
        public bool Locked { get; set; }
        public Guid? ChosenScenarioId { get; set; }
    }
}
