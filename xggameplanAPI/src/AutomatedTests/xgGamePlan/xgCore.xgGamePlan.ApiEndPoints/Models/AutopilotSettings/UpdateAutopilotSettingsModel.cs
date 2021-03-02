using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings
{
    public class UpdateAutopilotSettingsModel
    {
        public int Id { get; set; }
        public int DefaultFlexibilityLevelId { get; set; }
        public int ScenariosToGenerate { get; set; }
        public IEnumerable<UpdateAutopilotRuleModel> AutopilotRules { get; set; }
    }
}
