using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults
{
    public class ScenarioResultModel
    {
        public Guid Id { get; set; }
        public DateTime TimeCompleted { get; set; }
        public RunReference Run { get; set; }
        public IEnumerable<FailureModel> Failures { get; set; }
    }
}
