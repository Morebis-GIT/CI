using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace xggameplan.Model
{
    public class ScenarioCampaignResultModel
    {
        /// <summary>
        /// ExternalCampRef
        /// </summary>
        public string ExternalCampRef { get; set; }

        /// <summary>
        /// CampaignName
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// CampaignGroup
        /// </summary>
        public string CampaignGroup { get; set; }

        /// <summary>
        /// BusinessType
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// CampaignStartDate
        /// </summary>
        public DateTime CampaignStartDate { get; set; }

        /// <summary>
        /// CampaignEndDate
        /// </summary>
        public DateTime CampaignEndDate { get; set; }

        /// <summary>
        /// DaysToEndOfCampaign
        /// </summary>
        public int DaysToEndOfCampaign { get; set; }

        /// <summary>
        /// ClashName
        /// </summary>
        public string ChildClashName { get; set; }

        /// <summary>
        /// ParentClashName
        /// </summary>
        public string ParentClashName { get; set; }

        /// <summary>
        /// AgencyName
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// ProductName
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// AdvertiserName
        /// </summary>
        public string AdvertiserName { get; set; }

        /// <summary>
        /// DemographicName
        /// </summary>
        public string DemographicName { get; set; }

        /// <summary>
        /// SalesAreaGroupName
        /// </summary>
        public string SalesAreaGroupName { get; set; }

        /// <summary>
        /// StrikeWeightStartDate
        /// </summary>
        public DateTime StrikeWeightStartDate { get; set; }

        /// <summary>
        /// StrikeWeightEndDate
        /// </summary>
        public DateTime StrikeWeightEndDate { get; set; }

        /// <summary>
        /// DaypartName
        /// </summary>
        public string DaypartName { get; set; }

        /// <summary>
        /// DurationSecs
        /// </summary>
        public int DurationSecs { get; set; }

        /// <summary>
        /// TargetRatings
        /// </summary>
        public double TargetRatings { get; set; }

        /// <summary>
        /// PreRunRatings
        /// </summary>
        public double PreRunRatings { get; set; }

        /// <summary>
        /// PreRunRatingsDiff
        /// </summary>
        public double PreRunRatingsDiff { get; set; }

        /// <summary>
        /// ISRCancelledRatings
        /// </summary>
        public double ISRCancelledRatings { get; set; }

        /// <summary>
        /// ISRCancelledSpots
        /// </summary>
        public double ISRCancelledSpots { get; set; }

        /// <summary>
        /// RSCancelledRatings
        /// </summary>
        public double RSCancelledRatings { get; set; }

        /// <summary>
        /// RSCancelledSpots
        /// </summary>
        public double RSCancelledSpots { get; set; }

        /// <summary>
        /// OptimiserRatings
        /// </summary>
        public double OptimiserRatings { get; set; }

        /// <summary>
        /// OptimiserBookedSpots
        /// </summary>
        public double OptimiserBookedSpots { get; set; }

        /// <summary>
        /// ZeroRatedSpots
        /// </summary>
        public int? ZeroRatedSpots { get; set; }

        /// <summary>
        /// PostRunRatings
        /// </summary>
        public double PostRunRatings { get; set; }

        /// <summary>
        /// PostRunRatingsDiff
        /// </summary>
        public double PostRunRatingsDiff { get; set; }

        /// <summary>
        /// TargetAchievedPct
        /// </summary>
        public double TargetAchievedPct { get; set; }

        /// <summary>
        /// Gets or sets the media sales group.
        /// </summary>
        /// <value>
        /// The media sales group.
        /// </value>
        public string MediaSalesGroup { get; set; }

        /// <summary>
        /// Gets or sets the product assignee.
        /// </summary>
        /// <value>
        /// The product assignee.
        /// </value>
        public string ProductAssignee { get; set; }

        /// <summary>
        /// StopBooking.
        /// </summary>
        public bool StopBooking { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the automated booked.
        /// </summary>
        /// <value>
        /// The automated booked.
        /// </value>
        public bool? AutomatedBooked { get; set; }

        /// <summary>
        /// Gets or sets the Top/Tail.
        /// </summary>
        /// <value>
        /// The Top/Tail.
        /// </value>
        public TopTail? TopTail { get; set; }

        /// <summary>
        /// Gets or sets the reporting category.
        /// </summary>
        /// <value>
        /// The reporting category.
        /// </value>
        public string ReportingCategory { get; set; }

        /// <summary>
        /// Gets or sets the clash code.
        /// </summary>
        /// <value>
        /// The clash code.
        /// </value>
        public string ClashCode { get; set; }

        /// <summary>
        /// NominalValue
        /// </summary>
        public double? NominalValue { get; set; }

        /// <summary>
        /// TotalNominalValue
        /// </summary>
        public double? TotalNominalValue { get; set; }

        /// <summary>
        /// RevenueBudget
        /// </summary>
        public double? RevenueBudget { get; set; }

        /// <summary>
        /// TotalPayback
        /// </summary>
        public double? TotalPayback { get; set; }

        /// <summary>
        /// DifferenceValueDelivered
        /// </summary>
        public double? DifferenceValueDelivered { get; set; }

        /// <summary>
        /// DifferenceValueDeliveredPercentage
        /// </summary>
        public double? DifferenceValueDeliveredPercentage { get; set; }

        /// <summary>
        /// DifferenceValueDeliveredPayback
        /// </summary>
        public double? DifferenceValueDeliveredPayback { get; set; }

        /// <summary>
        /// DifferenceValueDeliveredPercentagePayback
        /// </summary>
        public double? DifferenceValueDeliveredPercentagePayback { get; set; }

        /// <summary>
        /// PassThatDelivered100Percent
        /// </summary>
        public int? PassThatDelivered100Percent { get; set; }
    }
}
