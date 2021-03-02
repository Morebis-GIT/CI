using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignCreatedOrUpdated : ICampaignCreatedOrUpdated
    {
        public CampaignCreatedOrUpdated(int customId, string externalId, string name, string demoGraphic, DateTime startDateTime,
            DateTime endDateTime, string product, double revenueBudget, decimal targetRatings, decimal actualRatings,
            string campaignGroup, bool isPercentage, string status, string businessType, string deliveryType,
            bool includeOptimisation, bool targetZeroRatedBreaks, bool inefficientSpotRemoval, string includeRightSizer,
            string expectedClearanceCode, int campaignPassPriority, List<string> breakType, int campaignSpotMaxRatings,
            List<SalesAreaCampaignTarget> salesAreaCampaignTarget, List<TimeRestriction> timeRestrictions,
            List<ProgrammeRestriction> programmeRestrictions, List<CampaignBookingPositionGroup> bookingPositionGroups,
            DeliveryCurrency deliveryCurrency, bool stopBooking, double? targetXp, double? revenueBooked, DateTime? creationDate,
            bool? automatedBooked, int? spots, List<CampaignPayback> campaignPaybacks, TopTail? topTail, CampaignBreakRequirement breakRequirement)
        {
            CustomId = customId;
            ExternalId = externalId;
            Name = name;
            DemoGraphic = demoGraphic;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Product = product;
            RevenueBudget = revenueBudget;
            TargetRatings = targetRatings;
            ActualRatings = actualRatings;
            CampaignGroup = campaignGroup;
            IsPercentage = isPercentage;
            Status = status;
            BusinessType = businessType;
            DeliveryType = deliveryType;
            IncludeOptimisation = includeOptimisation;
            TargetZeroRatedBreaks = targetZeroRatedBreaks;
            InefficientSpotRemoval = inefficientSpotRemoval;
            IncludeRightSizer = includeRightSizer;
            ExpectedClearanceCode = expectedClearanceCode;
            CampaignPassPriority = campaignPassPriority;
            BreakType = breakType;
            CampaignSpotMaxRatings = campaignSpotMaxRatings;
            SalesAreaCampaignTarget = salesAreaCampaignTarget;
            TimeRestrictions = timeRestrictions;
            ProgrammeRestrictions = programmeRestrictions;
            BookingPositionGroups = bookingPositionGroups;
            DeliveryCurrency = deliveryCurrency;
            StopBooking = stopBooking;
            TargetXP = targetXp;
            RevenueBooked = revenueBooked;
            CreationDate = creationDate;
            AutomatedBooked = automatedBooked;
            Spots = spots;
            CampaignPaybacks = campaignPaybacks;
            TopTail = topTail;
            BreakRequirement = breakRequirement;
        }
        public int CustomId { get; }
        public string ExternalId { get; }
        public string Name { get; }
        public string DemoGraphic { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public string Product { get; }
        public double RevenueBudget { get; }
        public decimal TargetRatings { get; }
        public decimal ActualRatings { get; }
        public string CampaignGroup { get; }
        public bool IsPercentage { get; }
        public string Status { get; }
        public string BusinessType { get; }
        public string DeliveryType { get; }
        public bool IncludeOptimisation { get; } = true;
        public bool TargetZeroRatedBreaks { get; }
        public bool InefficientSpotRemoval { get; } = true;
        public string IncludeRightSizer { get; }
        public string ExpectedClearanceCode { get; }
        public int CampaignPassPriority { get; }
        public int CampaignSpotMaxRatings { get; }
        public bool StopBooking { get; }
        public double? TargetXP { get; }
        public double? RevenueBooked { get; }
        public DateTime? CreationDate { get; }
        public bool? AutomatedBooked { get; }
        public TopTail? TopTail { get; }
        public int? Spots { get; }

        public List<string> BreakType { get; }
        public List<SalesAreaCampaignTarget> SalesAreaCampaignTarget { get; }
        public List<TimeRestriction> TimeRestrictions { get; }
        public List<ProgrammeRestriction> ProgrammeRestrictions { get; }
        public List<CampaignBookingPositionGroup> BookingPositionGroups { get; }
        public DeliveryCurrency DeliveryCurrency { get; }
        public List<CampaignPayback> CampaignPaybacks { get; }
        public CampaignBreakRequirement BreakRequirement { get; }
    }
}
