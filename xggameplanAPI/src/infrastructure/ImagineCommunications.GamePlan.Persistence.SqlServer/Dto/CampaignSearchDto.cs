using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using CampaignStatus = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.CampaignStatus;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Dto
{
    public class CampaignSearchDto
    {
        public Guid Uid { get; set; }
        public int CustomId { get; set; }
        public CampaignStatus Status { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public string CampaignGroup { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string ProductExternalRef { get; set; }
        public string ProductName { get; set; }
        public string AdvertiserName { get; set; }
        public string AgencyName { get; set; }
        public string MediaGroupCode { get; set; }
        public string MediaGroupShortName { get; set; }
        public string ReportingCategory { get; set; }
        public string ProductAssigneeName { get; set; }
        public string BusinessType { get; set; }
        public CampaignDeliveryType DeliveryType { get; set; }
        public string Demographic { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public bool IsPercentage { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool TargetZeroRatedBreaks { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public bool IncludeRightSizer { get; set; }
        public RightSizerLevel? RightSizerLevel { get; set; }
        public int CampaignPassPriority { get; set; }
        public string ClashCode { get; set; }
        public string ClashDescription { get; set; }
        public bool StopBooking { get; set; }
        public double? TargetXP { get; set; }
        public double? RevenueBooked { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? AutomatedBooked { get; set; }
        public TopTail? TopTail { get; set; }
        public int? Spots { get; set; }
        public ICollection<CampaignPayback> CampaignPaybacks { get; set; } = new HashSet<CampaignPayback>();
        public string ActiveLength { get; set; }
        public decimal? RatingsDifferenceExcludingPayback { get; set; }
        public decimal? ValueDifference { get; set; }
        public decimal? ValueDifferenceExcludingPayback { get; set; }
        public decimal? AchievedPercentageTargetRatings { get; set; }
        public decimal? AchievedPercentageRevenueBudget { get; set; }
    }
}
