using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class RemovedSponsoredSpotCountFromBreakEventArgs
        : RemovedSpotFromBreakEventArgs
    {
        public RemovedSponsoredSpotCountFromBreakEventArgs(
            BreakExternalReference breakExternalReference,
            ProductExternalReference productExternalReference,
            SponsorshipApplicability applicability,
            SponsorshipCalculationType calculationType)
            : base(
                  breakExternalReference,
                  advertiserIdentifier: string.Empty,
                  clashExternalReference: string.Empty,
                  productExternalReference,
                  true,
                  applicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotCount, Duration.Zero)
                  )
        { }
    }
}
