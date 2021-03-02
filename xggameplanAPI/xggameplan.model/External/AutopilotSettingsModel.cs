using System.Collections.Generic;

namespace xggameplan.Model
{
    public class AutopilotSettingsModel
    {
        public int Id { get; set; }
        public int DefaultFlexibilityLevelId { get; set; }
        public int ScenariosToGenerate { get; set; }
        public List<AutopilotRuleModel> AutopilotRules { get; set; }
    }
}
