using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using xggameplan.core.Extensions;

namespace xggameplan.KPIProcessing.KPICalculation.Infrastructure
{
    public static class KPICalculationHelpers
    {
        /// <summary>
        /// Collection of possible states of the Spot
        /// </summary>
        public static class SpotTags
        {
            /// <summary>
            /// Booked spot tag
            /// </summary>
            public const string Booked = "B";

            /// <summary>
            /// Cancelled spot tag
            /// </summary>
            public const string Cancelled = "C";

            /// <summary>
            /// Smoothed spot tag
            /// </summary>
            public const string Smoothed = "A";
        }

        /// <summary>
        /// Collection of possible KPI display formats
        /// </summary>
        public static class DisplayFormats
        {
            public const string LargeNumber = "largenumber";

            public const string Percentage = "percentage";
        }

        public static IEnumerable<DayPart> GetCampaignsDayParts(IEnumerable<Campaign> campaigns)
        {
            var result = new List<DayPart>();

            foreach (var campaign in campaigns)
            {
                result.AddRange(GetCampaignDayParts(campaign));
            }

            return result;
        }

        public static IEnumerable<DayPart> GetCampaignDayParts(Campaign campaign)
        {
            var strikeWeights = GetCampaignStrikeWeights(campaign);

            return strikeWeights.SelectMany(s => s.DayParts);
        }

        public static IEnumerable<StrikeWeight> GetCampaignStrikeWeights(Campaign campaign)
        {
            var campaignTargets = campaign.SalesAreaCampaignTarget.SelectMany(s => s.CampaignTargets);

            return campaignTargets.Where(t =>
                    t.StrikeWeights != null
                    && t.StrikeWeights.Any())
                .SelectMany(s => s.StrikeWeights)
                .ToArray();
        }

        public static IEnumerable<StrikeWeight> GetSalesAreaCampaignTargetStrikeWeights(SalesAreaCampaignTarget salesAreaCampaignTarget)
        {
            return salesAreaCampaignTarget.CampaignTargets.Where(t =>
                    t.StrikeWeights != null
                    && t.StrikeWeights.Any())
                .SelectMany(s => s.StrikeWeights)
                .ToArray();
        }

        public static decimal GetSmoothCampaignRatingPercentage(Campaign campaign,
            IDictionary<string, IEnumerable<RecommendationsByScenarioReduceResult>> campaignMetrics)
        {
            decimal delta = 0;

            if (campaignMetrics.TryGetValue(campaign.ExternalId, out var adjustmentList))
            {
                foreach (var adj in adjustmentList)
                {
                    delta = adj.Action == KPICalculationHelpers.SpotTags.Cancelled
                        ? delta - adj.SpotRating
                        : delta + adj.SpotRating;
                }
            }

            var percentage = (campaign.ActualRatings + delta) / campaign.TargetRatings;
            return percentage;
        }

        //This is temporary solution to identify DayPart by it's DOW and Time
        //Example of dowTimeString: "2100-2659(Mon-Fri)"
        //Will be removed in scope of technical debt (XGGT-16613) and DayParts will receive identifier guid
        public static DayPart GetDayPartByDowTimeString(List<DayPart> dayParts, string dowTimeString)
        {
            if (dayParts is null || dowTimeString is null
                || dowTimeString == "NotSupplied" || dowTimeString.Length < 18)
            {
                return null;
            }

            var AgFromTime = dowTimeString.Substring(0, 4);

            var AgToTime = dowTimeString.Substring(5, 4);

            var firstDOW = dowTimeString.Substring(10, 3);

            var lastDOW = dowTimeString.Substring(14, 3);

            var fromTime = AgConversions.ParseTotalHHMMSSFormat(AgFromTime + "00", false).ToString(@"hh\:mm");

            var toTime = AgConversions.ParseTotalHHMMSSFormat(AgToTime + "59", false).ToString(@"hh\:mm");

            return dayParts.Where(d
                => d.Timeslices.Any(t => t.FromTime == fromTime && t.ToTime == toTime
                && t.DowPattern.Contains(firstDOW) && t.DowPattern.Contains(lastDOW)))
                .FirstOrDefault();
        }
    }
}
