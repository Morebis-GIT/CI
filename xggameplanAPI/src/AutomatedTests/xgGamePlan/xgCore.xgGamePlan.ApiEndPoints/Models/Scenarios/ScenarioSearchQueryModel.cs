using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios
{
    public class ScenarioSearchQueryModel
    {
        public string Name { get; set; }
        public bool DefaultScenarioAtTopOfFirstPage { get; set; }
        public bool? IsLibraried { get; set; }
        public IEnumerable<Order<string>> OrderBy { get; set; }
    }
}
