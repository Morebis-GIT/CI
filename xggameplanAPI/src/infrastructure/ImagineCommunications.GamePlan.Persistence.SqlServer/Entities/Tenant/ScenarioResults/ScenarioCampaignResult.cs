using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults
{
    public class ScenarioCampaignResult : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }
        public string CampaignExternalId { get; set; }
        public string SalesAreaName { get; set; }
        public int SpotLength { get; set; }
        public DateTime StrikeWeightStartDate { get; set; }
        public DateTime StrikeWeightEndDate { get; set; }
        public string DaypartName { get; set; }
        public double TargetRatings { get; set; }
        public double PreRunRatings { get; set; }
        public double ISRCancelledRatings { get; set; }
        public double ISRCancelledSpots { get; set; }
        public double RSCancelledRatings { get; set; }
        public double RSCancelledSpots { get; set; }
        public double OptimiserRatings { get; set; }
        public double OptimiserBookedSpots { get; set; }
        public int PassThatDelivered100Percent { get; set; }

        public override bool Equals(object obj)
        {
            var second = obj as ScenarioCampaignResult;

            if (second == null)
            {
                return false;
            }

            if (CampaignExternalId != second.CampaignExternalId)
            {
                return false;
            }

            if (SalesAreaName != second.SalesAreaName)
            {
                return false;
            }

            if (SpotLength != second.SpotLength)
            {
                return false;
            }

            if (StrikeWeightStartDate != second.StrikeWeightStartDate)
            {
                return false;
            }

            if (StrikeWeightEndDate != second.StrikeWeightEndDate)
            {
                return false;
            }

            if (DaypartName != second.DaypartName)
            {
                return false;
            }

            if (TargetRatings != second.TargetRatings)
            {
                return false;
            }

            if (PreRunRatings != second.PreRunRatings)
            {
                return false;
            }

            if (ISRCancelledRatings != second.ISRCancelledRatings)
            {
                return false;
            }

            if (ISRCancelledSpots != second.ISRCancelledSpots)
            {
                return false;
            }

            if (RSCancelledRatings != second.RSCancelledRatings)
            {
                return false;
            }

            if (RSCancelledSpots != second.RSCancelledSpots)
            {
                return false;
            }

            if (OptimiserRatings != second.OptimiserRatings)
            {
                return false;
            }

            if (OptimiserBookedSpots != second.OptimiserBookedSpots)
            {
                return false;
            }

            if (PassThatDelivered100Percent != second.PassThatDelivered100Percent)
            {
                return false;
            }

            return true;
        }
    }
}
