using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SponsorshipRestrictionFilterResults
    {
        public ImmutableList<SponsoredItem> SponsoredItems { get; set; }
    }
}
