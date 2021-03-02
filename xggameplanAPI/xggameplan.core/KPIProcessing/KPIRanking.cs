using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using xggameplan.Model;

namespace xggameplan.KPIProcessing
{
    public class KPIRanking
    {
        public static void PerformRanking(List<ScenarioMetricsResultModel> results, List<KPIComparisonConfig> comparisonConfigs)
        {
            if (results.Count < 2)
            {
                return;
            }

            comparisonConfigs.Where(KPIComparison => KPIComparison.Ranked).ToList().ForEach(KPIComparison =>
            {
                List<ScenarioMetricsResultModel> resultsWithMetric = results.Where(x => x.Metrics.Find(y => y.Name == KPIComparison.KPIName) != null).ToList();

                if (resultsWithMetric.Count > 1)
                {
                    resultsWithMetric.Sort((a, b) =>
                    {
                        var aKPIValue = a.GetKPIValue(KPIComparison.KPIName);
                        var bKPIValue = b.GetKPIValue(KPIComparison.KPIName);

                        if (KPIComparison.HigherIsBest)
                        {
                            return aKPIValue.Value.CompareTo(bKPIValue.Value) * -1;
                        }
                        else
                        {
                            return aKPIValue.Value.CompareTo(bKPIValue.Value);
                        }
                    });

                    var firstValue = resultsWithMetric[0].GetKPIValue(KPIComparison.KPIName);
                    var lastValue = resultsWithMetric[resultsWithMetric.Count - 1].GetKPIValue(KPIComparison.KPIName);

                    if (Math.Abs(firstValue.Value - lastValue.Value) >= KPIComparison.DiscernibleDifference)
                    {
                        var resultGroups = GroupByDiscernableDifference(resultsWithMetric, KPIComparison);

                        for (int index = 0; index < resultGroups.Count; index++)
                        {
                            var newRanking = Model.KPIRanking.None;

                            if (index == 0 && resultGroups.Count > 1)
                            {
                                newRanking = Model.KPIRanking.Best;
                            }
                            else if (index == 1 && resultGroups.Count > 2)
                            {
                                newRanking = Model.KPIRanking.SecondBest;
                            }

                            resultGroups[index].ToList().ForEach(result =>
                            {
                                var metricValue = result.Metrics.FirstOrDefault(metric => metric.Name.Equals(KPIComparison.KPIName));
                                metricValue.Ranking = newRanking;
                            });
                        }
                    }
                }
            });
        }

        private static List<List<ScenarioMetricsResultModel>> GroupByDiscernableDifference(List<ScenarioMetricsResultModel> results, KPIComparisonConfig KPIComparison)
        {
            var resultGroups = new List<List<ScenarioMetricsResultModel>>();
            var currentGroup = new List<ScenarioMetricsResultModel>();
            resultGroups.Add(currentGroup);

            results.ForEach(item =>
            {
                if (currentGroup.Count() == 0 || IsDiscernablyDifferent(currentGroup.First(), item, KPIComparison))
                {
                    currentGroup.Add(item);
                }
                else
                {
                    currentGroup = new List<ScenarioMetricsResultModel>();
                    resultGroups.Add(currentGroup);
                    currentGroup.Add(item);
                }
            });

            return resultGroups;
        }

        private static bool IsDiscernablyDifferent(ScenarioMetricsResultModel a, ScenarioMetricsResultModel b, KPIComparisonConfig KPIComparison)
        {
            return Math.Abs(
                        a.GetKPIValue(KPIComparison.KPIName).Value -
                        b.GetKPIValue(KPIComparison.KPIName).Value)
                    < KPIComparison.DiscernibleDifference;
        }
    }
}
