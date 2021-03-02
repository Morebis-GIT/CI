using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace xggameplan.Model
{
    public class CampaignReportModel
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
        public bool IncludeOptimisation { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public bool IncludeRightSizer { get; set; }
        public RightSizerLevel? RightSizerLevel { get; set; }
        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }
        public int CampaignSpotMaxRatings { get; set; }
        public string ClashCode { get; set; }
        public string ClashDescription { get; set; }
        public string ActiveLength { get; set; }
        public decimal? RatingsDifferenceExcludingPayback { get; set; }
        public decimal? ValueDifference { get; set; }
        public decimal? ValueDifferenceExcludingPayback { get; set; }
        public decimal? AchievedPercentageTargetRatings { get; set; }
        public decimal? AchievedPercentageRevenueBudget { get; set; }
        public string ReportingCategory { get; set; }
        public string ProductAssignee { get; set; }
        public double? RevenueBooked { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? AutomatedBooked { get; set; }
        public TopTail? TopTail { get; set; }
        public int? Spots { get; set; }
        public bool StopBooking { get; set; }
        public string MediaSalesGroup { get; set; }
        public double? Payback { get; set; }
        public string PaybackType { get; set; }
    }
}
