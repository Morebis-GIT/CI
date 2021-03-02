using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class RemovedCompetitorSpotCountFromBreakEventArgs
        : RemovedSpotFromBreakEventArgs
    {
        public RemovedCompetitorSpotCountFromBreakEventArgs(
            BreakExternalReference breakExternalReference,
            string advertiserIdentifier,
            string clashExternalReference,
            ProductExternalReference productExternalReference,
            SponsorshipApplicability applicability,
            SponsorshipCalculationType calculationType)
            : base(
                  breakExternalReference,
                  advertiserIdentifier,
                  clashExternalReference,
                  productExternalReference,
                  false,
                  applicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotCount, Duration.Zero)
                  )
        { }
    }
}
