using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class RemovedSponsoredSpotDurationFromBreakEventArgs
        : RemovedSpotFromBreakEventArgs
    {
        public RemovedSponsoredSpotDurationFromBreakEventArgs(
            BreakExternalReference breakExternalReference,
            ProductExternalReference productExternalReference,
            SponsorshipApplicability applicability,
            SponsorshipCalculationType calculationType,
            Duration spotLength)
            : base(
                  breakExternalReference,
                  advertiserIdentifier: string.Empty,
                  clashExternalReference: string.Empty,
                  productExternalReference,
                  true,
                  applicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotDuration, spotLength)
                  )
        { }
    }
}
