using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings
{
    public class AutopilotScenarioEngageModel : Scenario
    {
        public int? LoosenPassIndex { get; set; }
        public int? TightenPassIndex { get; set; }
    }
}
