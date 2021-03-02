using System.Collections.Generic;

namespace xggameplan.Model
{
    public class UpdateAutopilotSettingsModel
    {
        public int Id { get; set; }
        public int DefaultFlexibilityLevelId { get; set; }
        public int ScenariosToGenerate { get; set; }
        public List<UpdateAutopilotRuleModel> AutopilotRules { get; set; }
    }
}
