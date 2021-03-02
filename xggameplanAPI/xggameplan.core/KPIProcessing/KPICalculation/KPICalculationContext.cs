using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.KPIProcessing.KPICalculation
{
    /// <inheritdoc />
    public class KPICalculationContext : IKPICalculationContext, IKPICache
    {
        private const string CancelledCampaignStatus = "C";

        /// <summary>
        /// Short names for demos against which according KPIs should be calculated.
        /// </summary>
        private static readonly HashSet<string> _kpiDemosToCalculate = new HashSet<string>
        {
            "ADS",
            "MN1634",
            "CHD",
            "HWCH",
            "ADABC1"
        };

        private static readonly HashSet<string> _reservedRatingsDemosToCalculate = new HashSet<string> { "ADS" };

        private readonly Dictionary<string, KPI> _kpiCache = new Dictionary<string, KPI>();

        public KPICalculationContext(IOutputDataSnapshot snapshot, RunWithScenarioReference runWithScenario)
        {
            ScenarioId = runWithScenario.ScenarioId;
            Snapshot = snapshot;
            ActiveCampaigns = new Lazy<IEnumerable<Campaign>>(GetActiveCampaigns, true);
            CampaignMetrics = new Lazy<IDictionary<string, IEnumerable<RecommendationsByScenarioReduceResult>>>(GetCampaignMetrics, true);
            SmoothCampaigns = new Lazy<IEnumerable<Campaign>>(GetSmoothCampaigns, true);
        }

        /// <inheritdoc />
        public Guid ScenarioId { get; }

        /// <inheritdoc />
        public IOutputDataSnapshot Snapshot { get; }

        /// <inheritdoc />
        public Lazy<IEnumerable<Campaign>> ActiveCampaigns { get; }

        /// <inheritdoc />
        public Lazy<IDictionary<string, IEnumerable<RecommendationsByScenarioReduceResult>>> CampaignMetrics { get; }

        /// <inheritdoc />
        public Lazy<IEnumerable<Campaign>> SmoothCampaigns { get; }

        /// <inheritdoc />
        public IEnumerable<string> FailureCampaignExternalIds { get; private set; }

        /// <inheritdoc />
        public IEnumerable<string> AvailableRatingsDemos { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> ReservedRatingsDemos { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> ConversionEfficiencyDemos { get; set; }

        /// <inheritdoc />
        public IEnumerable<Recommendation> Recommendations { get; set; }

        /// <inheritdoc />
        public IEnumerable<CampaignsReqm> CampaignLevels { get; set; }

        /// <inheritdoc />
        public IEnumerable<BaseRatings> BaseRatings { get; set; }

        /// <inheritdoc />
        public IEnumerable<ReserveRatings> ReserveRatings { get; set; }

        /// <inheritdoc />
        public IEnumerable<ConversionEfficiency> ConversionEfficiencies { get; set; }

        /// <inheritdoc />
        public IEnumerable<ScenarioCampaignLevelResultItem> ScenarioCampaignLevelResults { get; set; }

        /// <inheritdoc />
        public void SetContextData<TData>(TData data)
        {
            switch (data)
            {
                case SpotsReqmOutput spotFilesOutput:
                    Recommendations = spotFilesOutput.Recommendations;
                    break;

                case Failures failures:
                    FailureCampaignExternalIds = failures.Items
                        .Select(c => c.ExternalId)
                        .Distinct()
                        .ToList();
                    break;

                case CampaignsReqmOutput campaignLevels:
                    CampaignLevels = campaignLevels.Data;
                    break;

                case BaseRatingsOutput baseRatings:
                    BaseRatings = baseRatings.Data;
                    break;

                case ReserveRatingsOutput reserveRatings:
                    ReserveRatings = reserveRatings.Data;
                    break;

                case ConversionEfficiencyOutput conversionEfficiencies:
                    ConversionEfficiencies = conversionEfficiencies.Data;
                    break;

                case ScenarioCampaignLevelResult scenarioCampaignLevelResults:
                    ScenarioCampaignLevelResults = scenarioCampaignLevelResults.Items;
                    break;
            }
        }

        /// <inheritdoc />
        public void SetDefaultKpiDemos()
        {
            AvailableRatingsDemos = _kpiDemosToCalculate;
            ConversionEfficiencyDemos = _kpiDemosToCalculate;
            ReservedRatingsDemos = _reservedRatingsDemosToCalculate;
        }

        private IEnumerable<Campaign> GetActiveCampaigns()
        {
            var campaignsForScenario = Recommendations
                .Where(r => r.Action == KPICalculationHelpers.SpotTags.Booked)
                .Select(r => r.ExternalCampaignNumber)
                .Distinct()
                .ToArray();

            return Snapshot.AllCampaigns.Value.Where(c =>
                c.Status != CancelledCampaignStatus
                && campaignsForScenario.Contains(c.ExternalId)
                && c.TargetRatings != default);
        }

        private IDictionary<string, IEnumerable<RecommendationsByScenarioReduceResult>> GetCampaignMetrics() =>
            Recommendations
                .AsParallel()
                .GroupBy(x => Tuple.Create(x.ScenarioId, x.ExternalCampaignNumber, x.Action))
                .Select(agg => new RecommendationsByScenarioReduceResult
                {
                    ScenarioId = agg.Key.Item1,
                    ExternalCampaignNumber = agg.Key.Item2,
                    Action = agg.Key.Item3,
                    SpotRating = agg.Sum(x => x.SpotRating),
                    Count = agg.Count()
                })
                .GroupBy(c => c.ExternalCampaignNumber)
                .ToDictionary(c => c.Key, c => c.AsEnumerable());

        private IEnumerable<Campaign> GetSmoothCampaigns()
        {
            var campaignsForScenario = Recommendations
                .Where(x => x.Action != KPICalculationHelpers.SpotTags.Smoothed)
                .Select(_ => _.ExternalCampaignNumber)
                .Distinct()
                .ToArray();

            var combined = FailureCampaignExternalIds == null
                ? campaignsForScenario
                : campaignsForScenario.Union(FailureCampaignExternalIds);

            return Snapshot.AllCampaigns.Value
                .Where(c => IsCampaignActive(c)
                            && combined.Contains(c.ExternalId)
                            && c.TargetRatings != default)
                .ToArray();
        }

        private bool IsCampaignActive(Campaign c) =>
            c.Status != CancelledCampaignStatus &&
            (c.DeliveryType == CampaignDeliveryType.Spot
                ? c.TargetRatings >= default(decimal)
                : c.TargetZeroRatedBreaks || c.TargetRatings > default(decimal))
            && c.SalesAreaCampaignTarget.Any();

        public KPI GetKPI(string name) => _kpiCache.TryGetValue(name, out KPI result)
            ? result
            : null;

        public void AddKPIs(IEnumerable<KPI> kpiCollection)
        {
            foreach (var kpi in kpiCollection)
            {
                if (!_kpiCache.ContainsKey(kpi.Name))
                {
                    _kpiCache.Add(kpi.Name, kpi);
                }
            }
        }
    }
}
