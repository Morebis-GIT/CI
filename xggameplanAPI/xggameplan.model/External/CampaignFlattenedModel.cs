using System;

namespace xggameplan.Model
{
    public class CampaignFlattenedModel
    {
        public string ExternalCampaignRef { get; set; }
        public string CampaignName { get; set; }
        public string CampaignGroup { get; set; }
        public string BusinessType { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public DateTime CampaignEndDate { get; set; }
        public int DaysToEndOfCampign { get; set; }
        public decimal CampaignTargetRatings { get; set; }
        public decimal CampaignActualRatings { get; set; }
        public decimal CampaignTargetActualDiff { get; set; }
        public decimal CampaignTargetAchievedPct { get; set; }
        public string DemographicName { get; set; }
        public string AgencyName { get; set; }
        public string ProductName { get; set; }
        public string AdvertiserName { get; set; }
        public string ClashName { get; set; }
        public string ParentClashName { get; set; }
        public string SalesAreaGroupName { get; set; }
        public double SalesAreaGroupTargetRatings { get; set; }
        public double SalesAreaGroupActualRatings { get; set; }
        public double SalesAreaGroupTargetActualDiff { get; set; }
        public double SalesAreaGroupTargetAchievedPct { get; set; }
        public int DurationSecs { get; set; }
        public double DurationTargetRatings { get; set; }
        public double DurationActualRatings { get; set; }
        public double DurationTargetActualDiff { get; set; }
        public double DurationTargetAchievedPct { get; set; }
        public DateTime StrikeWeightStartDate { get; set; }
        public DateTime StrikeWeightEndDate { get; set; }
        public double StrikeWeightTargetRatings { get; set; }
        public double StrikeWeightActualRatings { get; set; }
        public double StrikeWeightTargetActualDiff { get; set; }
        public double StrikeWeightTargetAchievedPct { get; set; }
        public string DaypartTimeAndDays { get; set; }
        public double DaypartTargetRatings { get; set; }
        public double DaypartActualRatings { get; set; }
        public double DaypartTargetActualDiff { get; set; }
        public double DaypartTargetAchievedPct { get; set; }
    }
}
