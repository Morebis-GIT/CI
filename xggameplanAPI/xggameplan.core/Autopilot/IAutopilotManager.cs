using System.Collections.Generic;
using xggameplan.Model;

namespace xggameplan.Autopilot
{
    public interface IAutopilotManager
    {
        IList<AutopilotScenarioModel> Engage(AutopilotEngageModel command);
    }
}
