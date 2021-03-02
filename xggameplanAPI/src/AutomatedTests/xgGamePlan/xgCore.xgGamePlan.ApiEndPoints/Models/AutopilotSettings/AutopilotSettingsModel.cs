using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings
{
    public class AutopilotSettingsModel
    {
        public int Id { get; set; }
        public int DefaultFlexibilityLevelId { get; set; }
        public IEnumerable<AutopilotRuleModel> AutopilotRules { get; set; }
    }
}
