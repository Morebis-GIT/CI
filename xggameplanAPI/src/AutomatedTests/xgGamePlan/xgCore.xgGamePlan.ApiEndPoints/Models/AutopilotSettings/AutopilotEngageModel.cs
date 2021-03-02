using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings
{
    public class AutopilotEngageModel
    {
        public int FlexibilityLevelId { get; set; }
        public IEnumerable<AutopilotScenarioEngageModel> Scenarios { get; set; }
    }
}
