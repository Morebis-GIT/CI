using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.Model
{
    public class ScenarioMetricsResultModel
    {
        public Guid ScenarioId { get; set; }
        public DateTime TimeCompleted { get; set; }
        public double? BRSIndicator { get; set; }
        public List<KPIModel> Metrics { get; set; }
        public List<KPIModel> LandmarkMetrics { get; set; }
        public List<AnalysisGroupTargetMetricModel> AnalysisGroupMetrics { get; set; }

        public double? GetKPIValue(string KPIName)
        {
            if (Metrics == null)
            {
                return null;
            }

            return this.Metrics.FirstOrDefault(metric => metric.Name.Equals(KPIName)).Value;
        }
    }
}
