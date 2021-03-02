using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class SalesAreaGroup
    {
        public string GroupName { get; set; }
        public IEnumerable<string> SalesAreas { get; set; }
    }
}
