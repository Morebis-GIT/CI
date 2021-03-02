using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks
{
    public interface IBreakCreated : IEvent
    {
        DateTime ScheduledDate { get; }
        DateTime? BroadcastDate { get; }
        int? ClockHour { get; }
        string SalesArea { get; }
        string BreakType { get; }
        TimeSpan Duration { get; }
        TimeSpan ReserveDuration { get; }
        bool Optimize { get; }
        string ExternalBreakRef { get; }
        string Description { get; }
        string ExternalProgRef { get; }
        double BreakPrice { get; }
        double FloorRate { get; }
        BreakPosition PositionInProg { get; }

        /// <summary>
        /// Solus = All availability for a break is occupied by one spot
        /// </summary>
        bool Solus { get; }
        string PremiumCategory { get; }
        bool AllowSplit { get; }
        bool NationalRegionalSplit { get; }
        bool ExcludePackages { get; }
        bool BonusAllowed { get; }
        bool LongForm { get; }
    }
}
