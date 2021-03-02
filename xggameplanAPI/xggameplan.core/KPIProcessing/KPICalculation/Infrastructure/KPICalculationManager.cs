using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.KPIProcessing.Abstractions;

namespace xggameplan.KPIProcessing.KPICalculation.Infrastructure
{
    /// <inheritdoc />
    public class KPICalculationManager : IKPICalculationManager
    {
        private readonly IKPIResolver _kpiResolver;
        private readonly IAnalysisGroupKPIsCalculator _analysisGroupKpIsCalculator;
        private IAuditEventRepository _audit;
        private readonly bool _includeScenarioPerformanceMeasurementKpIs;

        public KPICalculationManager(IKPIResolver kpiResolver,
            IAnalysisGroupKPIsCalculator analysisGroupKpIsCalculator,
            IFeatureManager featureManager,
            IAuditEventRepository auditEventRepository)
        {
            _kpiResolver = kpiResolver;
            _analysisGroupKpIsCalculator = analysisGroupKpIsCalculator;
            _audit = auditEventRepository;

            _includeScenarioPerformanceMeasurementKpIs = featureManager.IsEnabled(nameof(ProductFeature.ScenarioPerformanceMeasurementKPIs));
        }

        /// <inheritdoc />
        public IKPICalculationManager SetAudit(IAuditEventRepository audit)
        {
            _audit = audit;
            return this;
        }

        /// <inheritdoc />
        public List<KPI> CalculateKPIs(Guid runId, Guid scenarioId)
        {
            var kpiNames = new HashSet<string>(_defaultKpiNamesToCalculate);
            if (_includeScenarioPerformanceMeasurementKpIs)
            {
                kpiNames.UnionWith(_scenarioPerformanceMeasurementKpIs);
            }

            return CalculateKPIs(kpiNames, runId, scenarioId);
        }

        /// <inheritdoc />
        public List<KPI> CalculateKPIs(HashSet<string> kpiNamesToCalculate, Guid rawRunId, Guid rawScenarioId)
        {
            if (kpiNamesToCalculate is null)
            {
                throw new ArgumentNullException(nameof(kpiNamesToCalculate));
            }

            var runId = rawRunId.ToString();
            var scenarioId = rawScenarioId.ToString();

            _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Executing KPI processing (RunID={runId}, ScenarioID={scenarioId}) "
                + $"KPIs to calculate: [{string.Join(", ", kpiNamesToCalculate)}]"));

            var kpis = new List<KPI>();

            foreach (var kpiName in kpiNamesToCalculate)
            {
                _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Executing KPI calculation (KPI={kpiName}, RunID={runId}, ScenarioID={scenarioId})"));

                try
                {
                    var kpiResult = _kpiResolver.Resolve(kpiName);

                    if (kpiResult is null || !kpiResult.Any())
                    {
                        _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            $"No result of {kpiName} KPI calculation (RunID={runId}, ScenarioID={scenarioId})"));

                        continue;
                    }

                    kpis.AddRange(kpiResult);
                }
                catch (DependencyResolutionException exception)
                {
                    _audit?.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        $"Error resolving calculator for {kpiName} KPI for Scenario {scenarioId} of Run {runId}",
                        exception));
                }
                catch (Exception exception)
                {
                    _audit?.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        $"Error calculating {kpiName} KPI for Scenario {scenarioId} of Run {runId}",
                        exception));
                }

                _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Executed KPI calculation (KPI={kpiName}, RunID={runId}, ScenarioID={scenarioId})"));
            }

            _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Executed KPI processing (RunID={runId}, ScenarioID={scenarioId})"));

            return kpis;
        }

        /// <inheritdoc />
        public List<AnalysisGroupTargetMetric> CalculateAnalysisGroupKPIs(Guid runId) => _analysisGroupKpIsCalculator.CalculateAnalysisGroupKPIs(runId);

        private static readonly IEnumerable<string> _defaultKpiNamesToCalculate = new string[]
        {
            ScenarioKPINames.AverageCancelEfficiency,
            ScenarioKPINames.AverageEfficiency,
            ScenarioKPINames.AvgSpotsPerDay,
            ScenarioKPINames.NoOfCampaigns,
            ScenarioKPINames.Percent75To95,
            ScenarioKPINames.Percent95To105,
            ScenarioKPINames.PercentBelow75,
            ScenarioKPINames.PercentGreater105,
            ScenarioKPINames.RatingCampaignsRatedSpots,
            ScenarioKPINames.RemainingAudience,
            ScenarioKPINames.RemainingAvailability,
            ScenarioKPINames.SpotCampaignsRatedSpots,
            ScenarioKPINames.StandardAverageCompletion,
            ScenarioKPINames.TotalRatingCampaignSpots,
            ScenarioKPINames.TotalSpotCampaignSpots,
            ScenarioKPINames.TotalSpotsBooked,
            ScenarioKPINames.TotalZeroRatedSpots,
            ScenarioKPINames.WeightedAverageCompletion
        };

        private static readonly IEnumerable<string> _scenarioPerformanceMeasurementKpIs = new string[]
        {
            ScenarioKPINames.AvailableRatingsByDemo,
            ScenarioKPINames.ReservedRatingsByDemo,
            ScenarioKPINames.TotalValueDelivered,
            ScenarioKPINames.TotalNominalValue,
            ScenarioKPINames.TotalPayback,
            ScenarioKPINames.TotalRevenue,
            ScenarioKPINames.BaseDemographicRatings,
            ScenarioKPINames.DifferenceValue,
            ScenarioKPINames.DifferenceValuePercentage,
            ScenarioKPINames.DifferenceValueWithPayback,
            ScenarioKPINames.DifferenceValuePercentagePayback,
            ScenarioKPINames.ConversionEfficiencyByDemo
        };
    }
}
