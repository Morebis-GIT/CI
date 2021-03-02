using System.Collections.Generic;

namespace xggameplan.Model
{
    public class AutopilotScenarioModel : CreateRunScenarioModelBase
    {
        public List<AutopilotPassModel> Passes { get; set; }
    }
}
