using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class ScenarioResultModel
    {
        public Guid Id { get; set; }

        public DateTime TimeCompleted { get; set; }

        public RunReference Run { get; set; }

        public double? BRSIndicator { get; set; }

        public List<FailureModel> Failures = new List<FailureModel>();
    }


}
