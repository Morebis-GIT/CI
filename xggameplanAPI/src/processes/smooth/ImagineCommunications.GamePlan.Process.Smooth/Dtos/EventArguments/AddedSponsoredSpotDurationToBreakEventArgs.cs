using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class AddedSponsoredSpotDurationToBreakEventArgs
        : AddedSpotToBreakEventArgs
    {
        public AddedSponsoredSpotDurationToBreakEventArgs(
            BreakExternalReference breakExternalReference,
            ProductExternalReference productExternalReference,
            SponsorshipApplicability applicability,
            SponsorshipCalculationType calculationType,
            Duration spotLength)
            : base(
                  breakExternalReference,
                  productExternalReference,
                  isSponsor: true,
                  productAdvertiserIdentifier: string.Empty,
                  productClashCode: string.Empty,
                  competitorType: SponsorshipCompetitorType.Neither,
                  applicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotDuration, spotLength)
                  )
        { }
    }
}
