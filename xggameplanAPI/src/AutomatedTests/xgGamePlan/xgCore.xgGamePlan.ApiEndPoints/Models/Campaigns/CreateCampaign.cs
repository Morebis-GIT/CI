using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class CreateCampaign
    {
        public Guid Uid { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string DemoGraphic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Product { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public string CampaignGroup { get; set; }
        public bool IsPercentage { get; set; }
        public string Status { get; set; }
        public string BusinessType { get; set; }
        public string DeliveryType { get; set; } = "Rating";
        public bool IncludeOptimisation { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public string IncludeRightSizer { get; set; }
        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }
        public IEnumerable<string> BreakType { get; set; }
        public IEnumerable<SalesAreaCampaignTarget> SalesAreaCampaignTarget { get; set; }
        public IEnumerable<TimeRestriction> TimeRestrictions { get; set; }
        public IEnumerable<ProgrammeRestriction> ProgrammeRestrictions { get; set; }
    }
}
