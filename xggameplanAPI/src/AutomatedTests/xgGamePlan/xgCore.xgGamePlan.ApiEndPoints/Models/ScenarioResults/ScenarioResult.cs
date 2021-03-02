using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults
{
    public class ScenarioResult
    {
        public Guid Id { get; set; }
        public DateTime TimeCompleted { get; set; }
        public IEnumerable<Kpi> Metrics { get; set; }
    }
}
