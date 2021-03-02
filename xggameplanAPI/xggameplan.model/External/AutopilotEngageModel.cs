using System.Collections.Generic;

namespace xggameplan.Model
{
    public class AutopilotEngageModel
    {
        public int FlexibilityLevelId { get; set; }
        public IEnumerable<AutopilotScenarioEngageModel> Scenarios { get; set; }
    }
}
