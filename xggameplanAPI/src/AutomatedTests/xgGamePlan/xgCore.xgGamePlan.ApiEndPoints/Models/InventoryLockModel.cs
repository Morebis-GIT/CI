using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models
{
    public class InventoryLockModel
    {
        public bool Locked { get; set; }
        public Guid? ChosenScenarioId { get; set; }
    }
}
