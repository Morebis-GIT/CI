using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot
{
    public readonly struct PivotBreak
    {
        public PivotBreak(
            IReadOnlyDictionary<string, Break> allBreaks,
            string externalBreakNo,
            string[] productIds)
        {
            Spots = new List<PivotSpot>();
            ExternalBreakNo = externalBreakNo;
            ProductIds = productIds;

            if (allBreaks.TryGetValue(externalBreakNo, out Break br))
            {
                BreakPosition = br.PositionInProg == Domain.Breaks.BreakPosition.C
                    ? "Center"
                    : "End";

                ScheduledDate = br.ScheduledDate;
                DurationInSecs = br.Duration.TotalSeconds;
                BreakType = br.BreakType;
            }
            else
            {
                BreakPosition = null;
                ScheduledDate = DateTime.MinValue;
                DurationInSecs = 0d;
                BreakType = null;
            }
        }

        public readonly DateTime ScheduledDate { get; }
        public readonly string BreakPosition { get; }
        public readonly double DurationInSecs { get; }
        public readonly string BreakType { get; }
        public readonly string ExternalBreakNo { get; }
        public readonly string[] ProductIds { get; }
        public readonly List<PivotSpot> Spots { get; }

        public readonly string ProductIdsStr => string.Join(",", ProductIds);

        public readonly string[] SpotByExternalReference => Spots.Select(s => s.ExternalSpotRef).ToArray();

        public override readonly string ToString() => ExternalBreakNo;

        public Recommendation Spot(string externalSpotRef)
        {
            if (Spots.Exists(s => string.Equals(s.ExternalSpotRef, externalSpotRef)))
            {
                return Spots
                        .Find(s => string.Equals(s.ExternalSpotRef, externalSpotRef))
                        .Recommendation;
            }

            return null;
        }
    }
}
