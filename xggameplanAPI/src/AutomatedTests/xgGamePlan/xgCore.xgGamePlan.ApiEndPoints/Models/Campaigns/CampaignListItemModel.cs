using System;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class CampaignListItemModel
    {
        public Guid Uid { get; set; }
        public int CustomId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public string CampaignGroup { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string ProductExternalRef { get; set; }
        public string ProductName { get; set; }
        public string AdvertiserName { get; set; }
        public string AgencyName { get; set; }
        public string BusinessType { get; set; }
        public string Demographic { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public bool IsPercentage { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public IncludeRightSizer IncludeRightSizer { get; set; }
        public CampaignPassPriorityType DefaultCampaignPassPriority { get; set; }
        public string ClashCode { get; set; }
        public string ClashDescription { get; set; }
    }
}
