using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class RemovedCompetitorSpotDurationFromBreakEventArgs
        : RemovedSpotFromBreakEventArgs
    {
        public RemovedCompetitorSpotDurationFromBreakEventArgs(
            BreakExternalReference breakExternalReference,
            string advertiserIdentifier,
            string clashExternalReference,
            ProductExternalReference productExternalReference,
            SponsorshipApplicability applicability,
            SponsorshipCalculationType calculationType,
            Duration spotLength)
            : base(
                  breakExternalReference,
                  advertiserIdentifier,
                  clashExternalReference,
                  productExternalReference,
                  false,
                  applicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotDuration, spotLength)
                  )
        { }
    }
}
