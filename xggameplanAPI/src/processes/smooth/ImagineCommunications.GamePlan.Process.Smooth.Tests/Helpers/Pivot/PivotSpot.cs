using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot
{
    public readonly struct PivotSpot
    {
        public PivotSpot(Recommendation recommendation)
        {
            Recommendation = recommendation;
            SpotDuration = recommendation.SpotLength.TotalSeconds;
            ActualPositionInBreak = recommendation.ActualPositionInBreak;
            RequestedPositionInBreak = recommendation.RequestedPositionInBreak;
            Product = recommendation.Product;
            Preemptlevel = recommendation.Preemptlevel;
            ExternalSpotRef = recommendation.ExternalSpotRef;
        }

        public readonly Recommendation Recommendation { get; }
        public readonly double SpotDuration { get; }
        public readonly string ExternalSpotRef { get; }
        public readonly string ActualPositionInBreak { get; }
        public readonly string RequestedPositionInBreak { get; }
        public readonly string Product { get; }
        public readonly int Preemptlevel { get; }

        public override readonly string ToString() =>
            $"{ExternalSpotRef} [Duration:{SpotDuration} sec, Product:{Product}, Preemptlevel:{Preemptlevel}, ActualPositionInBreak:{ActualPositionInBreak}, RequestedPositionInBreak:{RequestedPositionInBreak}]";
    }
}
