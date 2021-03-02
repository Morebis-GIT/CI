using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults
{
    public class GroupResult
    {
        public object Key { get; set; }
        public int Count { get; set; }
        public double Total { get; set; }
        public IEnumerable<GroupResult> SubGroups { get; set; }
    }
}
