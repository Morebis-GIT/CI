using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    public interface ISmoothSponsorshipTimelineManager
    {
        SmoothSponsorshipTimeline FindTimelineForCompetitor(
            BreakExternalReference breakExternalReference,
            string advertiserIdentifier,
            string clashExternalReference);

        SmoothSponsorshipTimeline FindTimelineForSponsoredProduct(
            BreakExternalReference breakExternalReference,
            string sponsoredProduct);

        void SetupTimelineRunningTotals(
            IImmutableList<Break> breaks);
    }
}
