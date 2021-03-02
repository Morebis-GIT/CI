using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.AuditEvents;
using xggameplan.core.Services;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation
{
    /// <inheritdoc />
    public class AnalysisGroupKPIsCalculator : IAnalysisGroupKPIsCalculator
    {
        private readonly IKPICalculationContext _kpiCalculationContext;
        private readonly IAnalysisGroupRepository _analysisGroupRepository;
        private readonly IAnalysisGroupCampaignQuery _analysisGroupCampaignQuery;
        private readonly IAuditEventRepository _auditEventRepository;

        public AnalysisGroupKPIsCalculator(IKPICalculationContext kpiCalculationContext,
            IAnalysisGroupRepository analysisGroupRepository,
            IAnalysisGroupCampaignQuery analysisGroupCampaignQuery,
            IAuditEventRepository auditEventRepository)
        {
            _kpiCalculationContext = kpiCalculationContext;
            _analysisGroupRepository = analysisGroupRepository;
            _analysisGroupCampaignQuery = analysisGroupCampaignQuery;
            _auditEventRepository = auditEventRepository;
        }

        /// <inheritdoc />
        public List<AnalysisGroupTargetMetric> CalculateAnalysisGroupKPIs(Guid runId)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Executing Run AnalysisGroup KPI processing (RunID={runId})"));

            var result = new List<AnalysisGroupTargetMetric>();

            var targets = _kpiCalculationContext.Snapshot.Run.Value.AnalysisGroupTargets;

            if (!targets.Any())
            {
                return result;
            }

            var analysisGroupTragets = targets
                .GroupBy(c => c.AnalysisGroupId);

            foreach (var analysisGroupTraget in analysisGroupTragets)
            {
                var analysisGroup = _analysisGroupRepository.GetIncludingSoftDeleted(analysisGroupTraget.Key);

                var campaignsIds = new HashSet<Guid>(_analysisGroupCampaignQuery.GetAnalysisGroupCampaigns(analysisGroup.Filter));
                var campaignsToInclude = _kpiCalculationContext.Snapshot
                    .AllCampaigns
                    .Value
                    .Where(c => campaignsIds.Contains(c.Id));

                var campaignExternalIds = new HashSet<string>(
                        campaignsToInclude.Select(c => c.ExternalId)
                    );

                var kpis = CalculateAnalysisGroupCampaignKPIs(
                        campaignsToInclude,
                        _kpiCalculationContext.Recommendations.Where(c => campaignExternalIds.Contains(c.ExternalCampaignNumber))
                    );

                foreach (var item in analysisGroupTraget)
                {
                    double value = default;

                    if (string.Equals(item.KPI, nameof(AnalysisGroupKPI.DeliveryPercentage), StringComparison.OrdinalIgnoreCase))
                    {
                        value = kpis.DeliveryPercentage;
                    }
                    else if (string.Equals(item.KPI, nameof(AnalysisGroupKPI.PoolValue), StringComparison.OrdinalIgnoreCase))
                    {
                        value = kpis.PoolValue;
                    }
                    else if (string.Equals(item.KPI, nameof(AnalysisGroupKPI.RatingsDelivery), StringComparison.OrdinalIgnoreCase))
                    {
                        value = kpis.RatingsDelivery;
                    }
                    else if (string.Equals(item.KPI, nameof(AnalysisGroupKPI.RevenueBooked), StringComparison.OrdinalIgnoreCase))
                    {
                        value = kpis.RevenueBooked;
                    }
                    else if (string.Equals(item.KPI, nameof(AnalysisGroupKPI.Spots), StringComparison.OrdinalIgnoreCase))
                    {
                        value = kpis.Spots;
                    }
                    else if (string.Equals(item.KPI, nameof(AnalysisGroupKPI.ZeroRatedSpots), StringComparison.OrdinalIgnoreCase))
                    {
                        value = kpis.ZeroRatedSpots;
                    }
                    else
                    {
                        continue;
                    }

                    var metric = new AnalysisGroupTargetMetric
                    {
                        AnalysisGroupTargetId = item.Id,
                        Value = value
                    };

                    result.Add(metric);
                }
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Executed Run AnalysisGroup KPI processing (RunID={runId})"));

            return result;
        }

        /// <summary>
        /// Calculates analysis group campaign KPIs
        /// </summary>
        /// <param name="campaigns"></param>
        /// <param name="campaignRecommendations"></param>
        /// <returns></returns>
        public static AnalysisGroupCampaignKPI CalculateAnalysisGroupCampaignKPIs(IEnumerable<Campaign> campaigns, IEnumerable<Recommendation> campaignRecommendations)
        {
            var bookedRecommendations = campaignRecommendations
                .Where(r => r.Action == KPICalculationHelpers.SpotTags.Booked);

            var cancelledRecommendations = campaignRecommendations
                .Where(r => r.Action == KPICalculationHelpers.SpotTags.Cancelled);

            var totalCampaignActualRatings = campaigns.Sum(c => c.ActualRatings);

            var ratingsDelivery = totalCampaignActualRatings +
                bookedRecommendations.Sum(r => r.SpotRating) -
                cancelledRecommendations.Sum(r => r.SpotRating);

            var totalCampaignTargetRatings = campaigns.Sum(c => c.TargetRatings);

            var deliveryPercentage = Math.Round(
                totalCampaignTargetRatings == 0
                    ? 0
                    : ratingsDelivery / totalCampaignTargetRatings * 100,
                2,
                MidpointRounding.AwayFromZero);

            var totalRecommendationsNominalPrice = bookedRecommendations.Sum(r => r.NominalPrice) -
                cancelledRecommendations.Sum(r => r.NominalPrice);

            var campaignRevenueBooked = campaigns.Sum(c => c.RevenueBooked);

            var revenueBooked = totalRecommendationsNominalPrice + campaignRevenueBooked.Value;

            var poolValue = revenueBooked - campaigns.Sum(c => c.RevenueBudget);

            var campaignDayParts = KPICalculationHelpers.GetCampaignsDayParts(campaigns);

            var spotsCount = campaignDayParts.Sum(d => d.TotalSpotCount)
                + bookedRecommendations.Count()
                - cancelledRecommendations.Count();

            var zeroRatedSpotsCount = campaignDayParts.Sum(d => d.ZeroRatedSpotCount)
                + bookedRecommendations.Count(x => x.SpotRating == 0)
                - cancelledRecommendations.Count(x => x.SpotRating == 0);

            return new AnalysisGroupCampaignKPI
            {
                RatingsDelivery = (double)ratingsDelivery,
                DeliveryPercentage = (double)deliveryPercentage,
                RevenueBooked = revenueBooked,
                PoolValue = poolValue,
                Spots = spotsCount,
                ZeroRatedSpots = zeroRatedSpotsCount
            };
        }
    }
}
