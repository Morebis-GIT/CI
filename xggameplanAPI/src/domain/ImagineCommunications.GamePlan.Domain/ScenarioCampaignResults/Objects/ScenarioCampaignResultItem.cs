using System;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects
{
    public class ScenarioCampaignResultItem
    {
        /// <summary>
        /// CampaignExternalId
        /// </summary>
        public string CampaignExternalId { get; set; }

        /// <summary>
        /// SalesAreaName
        /// </summary>
        public string SalesAreaName { get; set; }

        /// <summary>
        /// SpotLength
        /// </summary>
        public int SpotLength { get; set; }

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
        /// TargetRatings
        /// </summary>
        public double TargetRatings { get; set; }

        /// <summary>
        /// PreRunRatings
        /// </summary>
        public double PreRunRatings { get; set; }

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
        public double ZeroRatedSpots { get; set; }

        /// <summary>
        /// NominalValue
        /// </summary>
        public double NominalValue { get; set; }

        /// <summary>
        /// PassThatDelivered100Percent
        /// </summary>
        public int PassThatDelivered100Percent { get; set; }
    }
}
