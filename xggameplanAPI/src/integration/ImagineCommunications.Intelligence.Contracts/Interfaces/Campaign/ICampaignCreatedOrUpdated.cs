using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign
{
    public interface ICampaignCreatedOrUpdated : IEvent
    {
        string ExternalId { get; }

        int CustomId { get; }

        string Name { get; }

        string DemoGraphic { get; }

        DateTime StartDateTime { get; }

        DateTime EndDateTime { get; }

        string Product { get; }

        double RevenueBudget { get; }

        decimal TargetRatings { get; }

        decimal ActualRatings { get; }

        string CampaignGroup { get; }

        bool IsPercentage { get; }

        string Status { get; }

        string BusinessType { get; }

        string DeliveryType { get; }

        DeliveryCurrency DeliveryCurrency { get; }

        bool IncludeOptimisation { get; }

        bool TargetZeroRatedBreaks { get; }

        bool InefficientSpotRemoval { get; }

        string IncludeRightSizer { get; }

        string ExpectedClearanceCode { get; }

        int CampaignPassPriority { get; }

        int CampaignSpotMaxRatings { get; }

        bool StopBooking { get; }

        double? TargetXP { get; }

        double? RevenueBooked { get; }

        DateTime? CreationDate { get; }

        bool? AutomatedBooked { get; }

        TopTail? TopTail { get; }

        int? Spots { get; }

        List<string> BreakType { get; }

        List<SalesAreaCampaignTarget> SalesAreaCampaignTarget { get; }

        List<TimeRestriction> TimeRestrictions { get; }

        List<ProgrammeRestriction> ProgrammeRestrictions { get; }

        List<CampaignBookingPositionGroup> BookingPositionGroups { get; }

        List<CampaignPayback> CampaignPaybacks { get; }

        CampaignBreakRequirement BreakRequirement { get; }
    }
}
