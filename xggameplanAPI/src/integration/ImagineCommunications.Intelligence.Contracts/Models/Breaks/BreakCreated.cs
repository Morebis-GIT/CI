using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Breaks
{
    public class BreakCreated : IBreakCreated
    {
        public DateTime ScheduledDate { get; }

        public DateTime? BroadcastDate { get; }

        public int? ClockHour { get; }

        public string SalesArea { get; }

        public string BreakType { get; }

        public TimeSpan Duration { get; }

        public TimeSpan ReserveDuration { get; }

        public bool Optimize { get; }

        public string ExternalBreakRef { get; }

        public string Description { get; }

        public string ExternalProgRef { get; }

        public double BreakPrice { get; }

        public double FloorRate { get; }

        public BreakPosition PositionInProg { get; }

        public bool Solus { get; }

        public string PremiumCategory { get; }

        public bool AllowSplit { get; }

        public bool NationalRegionalSplit { get; }

        public bool ExcludePackages { get; }

        public bool BonusAllowed { get; }

        public bool LongForm { get; }

        public BreakCreated(
            DateTime scheduledDate,
            DateTime? broadcastDate,
            int? clockHour,
            string salesArea,
            string breakType,
            TimeSpan duration,
            TimeSpan reserveDuration,
            bool optimize,
            string externalBreakRef,
            string description,
            string externalProgRef,
            double breakPrice,
            double floorRate,
            BreakPosition positionInProg,
            bool solus,
            string premiumCategory,
            bool allowSplit,
            bool nationalRegionalSplit,
            bool excludePackages,
            bool bonusAllowed,
            bool longForm)
        {
            ScheduledDate = scheduledDate;
            BroadcastDate = broadcastDate;
            ClockHour = clockHour;
            SalesArea = salesArea;
            BreakType = breakType;
            Duration = duration;
            ReserveDuration = reserveDuration;
            Optimize = optimize;
            ExternalBreakRef = externalBreakRef;
            Description = description;
            ExternalProgRef = externalProgRef;
            BreakPrice = breakPrice;
            FloorRate = floorRate;
            PositionInProg = positionInProg;
            Solus = solus;
            PremiumCategory = premiumCategory;
            AllowSplit = allowSplit;
            NationalRegionalSplit = nationalRegionalSplit;
            ExcludePackages = excludePackages;
            BonusAllowed = bonusAllowed;
            LongForm = longForm;
        }
    }
}
