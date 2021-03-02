using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Runs
{
    public class ScenarioMetricsResultModel
    {
        public Guid ScenarioId { get; set; }

        public DateTime TimeCompleted { get; set; }

        public IEnumerable<KPIModel> Metrics { get; set; }
    }
}
