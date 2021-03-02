using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Represents context of the KPI calculation process.
    /// </summary>
    public interface IKPICalculationContext
    {
        /// <summary>
        /// Gets the scenario identifier for the current calculation context.
        /// </summary>
        /// <value>
        /// The scenario identifier.
        /// </value>
        Guid ScenarioId { get; }

        /// <summary>
        /// Snapshot of the data for the current calculation context.
        /// </summary>
        /// <value>
        /// The snapshot.
        /// </value>
        IOutputDataSnapshot Snapshot { get; }

        /// <summary>
        /// Gets active campaigns with Spot delivery type essential for the following
        /// KPIs calculation: Base demographic ratings, total number of spots for rating campaigns,
        /// total number of spots for spot campaigns and number of spots with non-zero rating for rating campaigns
        /// </summary>
        Lazy<IEnumerable<Campaign>> ActiveCampaigns { get; }

        /// <summary>
        /// Dictionary of campaign-specific metrics essential for the calculation process
        /// of the following "Percentage" KPIs: Below 75%, between 75% and 95%, between 95% and 105% and greater than 105%
        /// </summary>
        Lazy<IDictionary<string, IEnumerable<RecommendationsByScenarioReduceResult>>> CampaignMetrics { get; }

        /// <summary>
        /// Gets campaigns essential for the calculation process of the following
        /// "Percentage" KPIs: Below 75%, between 75% and 95%, between
        /// 95% and 105% and greater than 105%
        /// </summary>
        Lazy<IEnumerable<Campaign>> SmoothCampaigns { get; }

        /// <summary>
        /// Gets IDs of campaigns which have failures for current scenario
        /// </summary>
        IEnumerable<string> FailureCampaignExternalIds { get; }

        /// <summary>
        /// Set of demos against which "Available ratings for the specific demo" KPIs calculated
        /// </summary>
        IEnumerable<string> AvailableRatingsDemos { get; set; }

        /// <summary>
        /// Set of demos against which "Reserved ratings for the specific demo" KPIs calculated
        /// </summary>
        IEnumerable<string> ReservedRatingsDemos { get; set; }

        /// <summary>
        /// Set of demos against which "Conversion efficiency for the specific demo" KPIs calculated
        /// </summary>
        IEnumerable<string> ConversionEfficiencyDemos { get; set; }

        /// <summary>
        /// Set of recommendations for the scenario, essential for the majority of KPIs
        /// </summary>
        IEnumerable<Recommendation> Recommendations { get; set; }

        /// <summary>
        /// Set of campaign-specific data essential for the following KPIs calculation:
        /// Standard average completion and weighted average completion
        /// </summary>
        IEnumerable<CampaignsReqm> CampaignLevels { get; set; }

        /// <summary>
        /// Set of base ratings essential for the following KPIs calculation:
        /// Remaining audience and remaining availability
        /// </summary>
        IEnumerable<BaseRatings> BaseRatings { get; set; }

        /// <summary>
        /// Set of reserve ratings essential for the following KPIs calculation:
        /// Available ratings for the specific demo and reserved ratings for the specific demo
        /// </summary>
        IEnumerable<ReserveRatings> ReserveRatings { get; set; }

        /// <summary>
        /// Set of conversion efficiency essential for the following KPIs calculation:
        /// Conversion efficiency for the specific demo
        /// </summary>
        IEnumerable<ConversionEfficiency> ConversionEfficiencies { get; set; }

        /// <summary>
        /// Sets data into context if type is supported
        /// </summary>
        /// <typeparam name="TData">Type of data to be set.</typeparam>
        /// <param name="data"></param>
        void SetContextData<TData>(TData data);

        /// <summary>
        /// Sets default demos list for ScenarioPerformanceMeasurementKPIs calculation
        /// </summary>
        void SetDefaultKpiDemos();
    }
}
