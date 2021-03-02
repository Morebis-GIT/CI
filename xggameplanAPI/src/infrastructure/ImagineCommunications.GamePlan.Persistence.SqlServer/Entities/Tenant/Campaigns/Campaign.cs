using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class Campaign : IUniqueIdentifierPrimaryKey
    {
        public const string SearchTokensFieldName = "SearchTokens";

        public Guid Id { get; set; }
        public int CustomId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Demographic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Product { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public string CampaignGroup { get; set; }
        public bool IsPercentage { get; set; }
        public CampaignStatus Status { get; set; }
        public CampaignDeliveryType DeliveryType { get; set; }
        public CampaignDeliveryCurrency DeliveryCurrency { get; set; }
        public string BusinessType { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool TargetZeroRatedBreaks { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public bool IncludeRightSizer { get; set; }
        public RightSizerLevel? RightSizerLevel { get; set; }
        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }
        public int CampaignSpotMaxRatings { get; set; }
        public CampaignBreakRequirement BreakRequirement { get; set; }
        public bool StopBooking { get; set; }
        public string ActiveLength { get; set; }
        public decimal? RatingsDifferenceExcludingPayback { get; set; }
        public decimal? ValueDifference { get; set; }
        public decimal? ValueDifferenceExcludingPayback { get; set; }
        public decimal? AchievedPercentageTargetRatings { get; set; }
        public decimal? AchievedPercentageRevenueBudget { get; set; }
        public double? TargetXP { get; set; }
        public double? RevenueBooked { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? AutomatedBooked { get; set; }
        public TopTail? TopTail { get; set; }
        public int? Spots { get; set; }

        public ICollection<CampaignBreakType> BreakTypes { get; set; } =
            new HashSet<CampaignBreakType>();
        public ICollection<CampaignProgrammeRestriction> ProgrammeRestrictions { get; set; } =
            new HashSet<CampaignProgrammeRestriction>();
        public ICollection<CampaignTimeRestriction> TimeRestrictions { get; set; } =
            new HashSet<CampaignTimeRestriction>();
        public ICollection<CampaignSalesAreaTarget> SalesAreaCampaignTargets { get; set; } =
            new HashSet<CampaignSalesAreaTarget>();
        public ICollection<CampaignBookingPositionGroup> BookingPositionGroups { get; set; } =
            new HashSet<CampaignBookingPositionGroup>();
        public ICollection<CampaignPayback> CampaignPaybacks { get; set; } =
            new HashSet<CampaignPayback>();
    }
}
