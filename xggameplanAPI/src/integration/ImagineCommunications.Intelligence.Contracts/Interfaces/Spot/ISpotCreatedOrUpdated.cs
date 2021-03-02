using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot
{
    public interface ISpotCreatedOrUpdated : IEvent
    {
        string ExternalCampaignNumber { get; }

        string SalesArea { get; }

        string GroupCode { get; }

        string ExternalSpotRef { get; }

        DateTime StartDateTime { get; }

        DateTime EndDateTime { get; }

        TimeSpan SpotLength { get; }

        string BreakType { get; }

        string Product { get; }

        string Demographic { get; }

        bool ClientPicked { get; }

        string MultipartSpot { get; }

        string MultipartSpotPosition { get; }

        string MultipartSpotRef { get; }

        string RequestedPositioninBreak { get; }

        string ActualPositioninBreak { get; }

        string BreakRequest { get; }

        string ExternalBreakNo { get; }

        bool Sponsored { get; }

        bool Preemptable { get; }

        int Preemptlevel { get; }

        int BookingPosition { get; }

        string IndustryCode { get; }

        string ClearanceCode { get; }

        decimal NominalPrice { get; }
    }
}
