using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    public class SmoothSponsorshipTimelineManager : ISmoothSponsorshipTimelineManager
    {
        private readonly IReadOnlyList<SmoothSponsorshipTimeline> _timelines =
            new List<SmoothSponsorshipTimeline>();

        public SmoothSponsorshipTimelineManager(
            IReadOnlyList<SmoothSponsorshipTimeline> timelines)
        {
            _timelines = timelines;
        }

        public SmoothSponsorshipTimeline FindTimelineForSponsoredProduct(
            BreakExternalReference breakExternalReference,
            string sponsoredProduct) =>
            _timelines.FirstOrDefault(t =>
            t.BreakExternalReferences
            .Contains((string)breakExternalReference) &&
            t.SponsoredProducts
            .Contains(sponsoredProduct));

        public SmoothSponsorshipTimeline FindTimelineForCompetitor(
            BreakExternalReference breakExternalReference,
            string advertiserIdentifier,
            string clashExternalReference) =>
            _timelines.FirstOrDefault(t =>
            t.BreakExternalReferences
            .Contains((string)breakExternalReference) &&
            (t.AdvertiserIdentifiers
            .Select(a => a.advertiserIdentifier)
            .Contains(advertiserIdentifier) ||
            t.ClashExternalReferences
            .Select(c => c.clashExternalReference)
            .Contains(clashExternalReference)));

        public void SetupTimelineRunningTotals(
            IImmutableList<Break> breaks)
        {
            foreach (var timeline in _timelines)
            {
                if (timeline.CalculationType == SponsorshipCalculationType.None ||
                    timeline.CalculationType == SponsorshipCalculationType.Exclusive)
                {
                    continue;
                }

                foreach (var advertiser in timeline.AdvertiserIdentifiers)
                {
                    timeline.RunningTotals
                    .SetRestrictionValueForAdvertiserIdentifier(
                        advertiser.advertiserIdentifier,
                        advertiser.restrictionValue);
                }
                foreach (var clash in timeline.ClashExternalReferences)
                {
                    timeline.RunningTotals
                    .SetRestrictionValueForClashCode(
                        clash.clashExternalReference,
                        clash.restrictionValue);
                }

                timeline.RestrictionLimits = SponsorshipLimitsCalculator.CalculateRestrictionLimits(
                    timeline.RunningTotals,
                    timeline.CalculationType,
                    timeline.Applicability);

                timeline.BreakExternalReferences =
                    breaks.Where(b =>
                    timeline.DateTimeRange
                    .Overlays(
                        (
                        b.ScheduledDate,
                        b.ScheduledDate
                        .Add(
                            b.Duration.ToTimeSpan()
                            )
                        )
                        )
                    )
                    .Select(b => b.ExternalBreakRef)
                    .ToList();
            }
        }
    }
}
