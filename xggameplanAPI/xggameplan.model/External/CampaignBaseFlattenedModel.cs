using System;

namespace xggameplan.Model
{
    public class CampaignBaseFlattenedModel
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
    }
}
