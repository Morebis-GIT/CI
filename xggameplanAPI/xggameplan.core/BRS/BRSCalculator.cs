using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.core.Helpers;

namespace xggameplan.core.BRS
{
    public class BRSCalculator : IBRSCalculator
    {
        public IEnumerable<ScenarioResult> Calculate(
            IEnumerable<BRSConfigurationForKPI> kpiConfigurations,
            IEnumerable<ScenarioResult> scenarioResults,
            IEnumerable<KPIPriority> kpiPriorities)
        {
            var configurations = kpiConfigurations
                .Join(
                    kpiPriorities,
                    x => x.PriorityId,
                    y => y.Id,
                    (x, y) => new KPIWithWeightingFactor
                    {
                        KPIName = x.KPIName,
                        WeightingFactor = y.WeightingFactor
                    }
                );

            var allMetrics = scenarioResults.Select(x => x.Metrics);
            var bestResultIndicators = new double[scenarioResults.Count()].AsEnumerable();

            // Considering KPIs with weight factor 0 don't influence the results, they are excluded from evaluation completely
            foreach (var config in configurations.Where(x => x.WeightingFactor != 0))
            {
                var kpiMetrics = allMetrics.Select(x => x.First(y => y.Name == config.KPIName));

                var maxVal = kpiMetrics.Max(x => x.Value);
                var minVal = kpiMetrics.Min(x => x.Value);

                if (minVal == maxVal)
                {
                    continue;
                }

                var averageKPIVal = kpiMetrics.Average(x => x.Value);
                var delta = maxVal - minVal;

                var func = BRSHelper.CalculationFunctions[config.KPIName];
                var resultValues = kpiMetrics.Select(kpi => func(kpi.Value, averageKPIVal, delta, config.WeightingFactor)).ToList();

                bestResultIndicators = resultValues.Zip(bestResultIndicators, (x, y) => x + y);
            }

            return scenarioResults.Zip(bestResultIndicators, (scenarioResult, bestResultIndicator) =>
            {
                scenarioResult.BRSIndicator = Math.Round(bestResultIndicator, 2, MidpointRounding.AwayFromZero);
                return scenarioResult;
            });
        }
    }
}
