using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SmoothSponsorshipTimeline
    {
        public IEnumerable<string> BreakExternalReferences { get; set; }

        public IReadOnlyCollection<(string advertiserIdentifier, int restrictionValue)> AdvertiserIdentifiers { get; set; }
        public IReadOnlyCollection<(string clashExternalReference, int restrictionValue)> ClashExternalReferences { get; set; }
        public DateTimeRange DateTimeRange { get; set; }
        public IEnumerable<string> SponsoredProducts { get; set; }
        public SponsorshipApplicability Applicability { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }

        public SmoothSponsorshipRestrictionLimits RestrictionLimits { get; set; } =
            new SmoothSponsorshipRestrictionLimits();
        public SmoothSponsorshipRunningTotals RunningTotals { get; set; } =
            new SmoothSponsorshipRunningTotals();
    }
}
