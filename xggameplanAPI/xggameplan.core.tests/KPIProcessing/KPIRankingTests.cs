using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using NUnit.Framework;
using xggameplan.Model;

namespace xggameplan.KPIProcessing.tests
{
    [TestFixture]
    public class KPIRankingTests
    {
        //****** Cases from https://imaginecommunications.atlassian.net/wiki/spaces/XGG/pages/472219709/As+an+Airtime+Manager+viewing+a+completed+run+I+want+to+compare+the+individual+KPI+s+across+the+scenarios+so+that+I+can+easily+determine+best+and+second+best+in+class ***//

        [Test]
        public void PerformRanking_OneScenario_NoRanking()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(
                    scenarioId1,
                    new Dictionary<string,double>() {{ kpiName, 20 }})
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig() {
                    DiscernibleDifference = 5,
                    HigherIsBest = true,
                    KPIName = kpiName
                }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult = metricResults
                .Find(x => x.ScenarioId == scenarioId1)
                .Metrics
                .Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_TwoScenariosSameValues_NoRanking()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_TwoScenariosOneHigherValue_RanksFirstOnly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_TwoScenariosOneHigherValueRankingDisabled_NoRanking()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName, Ranked = false}
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenariosSameValue_NoRanking()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenariosFractionalDiscernableDifference_RanksCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 19.6 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 19.5 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 0.5f, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenariosOneHigherTwoEqualValues_OneValueRankedBest()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 40 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 30 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenariosThreeDifferentValues_RankedCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 40 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_FourScenariosSameValue_NoRanking()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();
            var scenarioId4 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId4, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var metricResult4 = metricResults.Find(x => x.ScenarioId == scenarioId4).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult4.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_FourScenariosTwoDifferentValues_RankedCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();
            var scenarioId4 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId4, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var metricResult4 = metricResults.Find(x => x.ScenarioId == scenarioId4).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(metricResult4.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_FourScenariosThreeDifferentValues_RankedCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();
            var scenarioId4 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 20 } }),
                CreateMetricsResult(scenarioId4, new Dictionary<string,double>() { { kpiName, 10 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var metricResult4 = metricResults.Find(x => x.ScenarioId == scenarioId4).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(metricResult4.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_FourScenariosThreeDifferentValues_RankedCorrectly2()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();
            var scenarioId4 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 40 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 40 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 30 } }),
                CreateMetricsResult(scenarioId4, new Dictionary<string,double>() { { kpiName, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var metricResult4 = metricResults.Find(x => x.ScenarioId == scenarioId4).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(metricResult4.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenariosThreeDifferentValuesOneLessThenDiscernableDifference_RankedCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 40 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 39 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 34 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_FractionalRankingGroups_RankedCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();
            var scenarioId4 = Guid.NewGuid();
            var scenarioId5 = Guid.NewGuid();
            var scenarioId6 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 40 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 39.5 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 39.1 } }),
                CreateMetricsResult(scenarioId4, new Dictionary<string,double>() { { kpiName, 39 } }),
                CreateMetricsResult(scenarioId5, new Dictionary<string,double>() { { kpiName, 38.5 } }),
                CreateMetricsResult(scenarioId6, new Dictionary<string,double>() { { kpiName, 34 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = true, KPIName = kpiName   }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var metricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var metricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var metricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var metricResult4 = metricResults.Find(x => x.ScenarioId == scenarioId4).Metrics.Find(x => x.Name == kpiName);
            var metricResult5 = metricResults.Find(x => x.ScenarioId == scenarioId5).Metrics.Find(x => x.Name == kpiName);
            var metricResult6 = metricResults.Find(x => x.ScenarioId == scenarioId6).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(metricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult3.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(metricResult4.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(metricResult5.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(metricResult6.Ranking == Model.KPIRanking.None);
        }

        //***** Additional Cases

        [Test]
        public void PerformRanking_ThreeScenario_1DiscerniblyDifferentMetric_LowerIsBest_RanksCorrectly()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() {{ kpiName, 2 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() {{ kpiName, 3 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() {{ kpiName, 1 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = false, KPIName = kpiName   },
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = true, KPIName = "kpi2"    }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var best = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var secondBest = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var thirdBest = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(best.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(secondBest.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(thirdBest.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_KPIValueRangeNotGreaterThenDiscernableDifference_AllResultsHaveNoRank()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() {{ kpiName, 3 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() {{ kpiName, 1 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() {{ kpiName, 2 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = false, KPIName = kpiName   },
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = true, KPIName = "kpi2"    }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var best = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var equalBest = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var thirdBest = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(best.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(equalBest.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(thirdBest.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenarios_2DiscerniblyDifferentMetrics_RanksCorrectly()
        {
            const string kpiName = "KPI1";
            const string kpi2Name = "KPI2";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 2 }, { kpi2Name, 30 } } ),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 3 }, { kpi2Name, 5  } } ),
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 1 }, { kpi2Name, 15 } } )
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = true, KPIName = kpiName   },
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = false, KPIName = kpi2Name    }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var KPI1best = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var KPI1secondBest = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var KPI1thirdBest = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);

            var KPI2best = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpi2Name);
            var KPI2secondBest = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpi2Name);
            var KPI2thirdBest = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpi2Name);

            Assert.IsTrue(KPI1best.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(KPI1secondBest.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(KPI1thirdBest.Ranking == Model.KPIRanking.None);

            Assert.IsTrue(KPI2best.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(KPI2secondBest.Ranking == Model.KPIRanking.SecondBest);
            Assert.IsTrue(KPI2thirdBest.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenarios_OneScenarioIsMissingMetricResult_RanksCorrectlyWithoutError()
        {
            const string kpiName = "KPI1";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() {{ kpiName, 1 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() {{ kpiName, 2 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>())
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = true, KPIName = kpiName   },
                new KPIComparisonConfig()   {   DiscernibleDifference = 1, HigherIsBest = true, KPIName = "kpi2"    }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var best = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var secondBest = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);

            Assert.IsTrue(best.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(secondBest.Ranking == Model.KPIRanking.None);
        }

        [Test]
        public void PerformRanking_ThreeScenariosIncludingZeroAndFractionalValue_RanksCorrectly()
        {
            const string kpiName = "KPI1";
            const string kpi2Name = "KPI2";
            var scenarioId1 = Guid.NewGuid();
            var scenarioId2 = Guid.NewGuid();
            var scenarioId3 = Guid.NewGuid();

            var metricResults = new List<ScenarioMetricsResultModel>()
            {
                CreateMetricsResult(scenarioId1, new Dictionary<string,double>() { { kpiName, 1 },      { kpi2Name, 5 } }),
                CreateMetricsResult(scenarioId2, new Dictionary<string,double>() { { kpiName, 45.3 },   { kpi2Name, 0.5 } }),
                CreateMetricsResult(scenarioId3, new Dictionary<string,double>() { { kpiName, 0 },      { kpi2Name, 20 } })
            };

            var comparisonConfigs = new List<KPIComparisonConfig>()
            {
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = true, KPIName = kpiName   },
                new KPIComparisonConfig()   {   DiscernibleDifference = 5, HigherIsBest = false, KPIName = kpi2Name }
            };

            KPIRanking.PerformRanking(metricResults, comparisonConfigs);

            var kpi1MetricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpiName);
            var kpi1MetricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpiName);
            var kpi1MetricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpiName);
            var kpi2MetricResult1 = metricResults.Find(x => x.ScenarioId == scenarioId1).Metrics.Find(x => x.Name == kpi2Name);
            var kpi2MetricResult2 = metricResults.Find(x => x.ScenarioId == scenarioId2).Metrics.Find(x => x.Name == kpi2Name);
            var kpi2MetricResult3 = metricResults.Find(x => x.ScenarioId == scenarioId3).Metrics.Find(x => x.Name == kpi2Name);

            Assert.IsTrue(kpi1MetricResult1.Ranking == Model.KPIRanking.None);
            Assert.IsTrue(kpi1MetricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(kpi1MetricResult3.Ranking == Model.KPIRanking.None);

            Assert.IsTrue(kpi2MetricResult1.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(kpi2MetricResult2.Ranking == Model.KPIRanking.Best);
            Assert.IsTrue(kpi2MetricResult3.Ranking == Model.KPIRanking.None);
        }

        private ScenarioMetricsResultModel CreateMetricsResult(Guid scenarioId, Dictionary<string, double> values)
        {
            var model = new ScenarioMetricsResultModel()
            {
                ScenarioId = scenarioId,
                TimeCompleted = DateTime.Now,
                Metrics = new List<KPIModel>()
            };

            foreach (KeyValuePair<string, double> kv in values)
            {
                model.Metrics.Add(new KPIModel()
                {
                    Name = kv.Key,
                    DisplayFormat = "",
                    Value = kv.Value
                });
            }
            return model;
        }
    }
}
