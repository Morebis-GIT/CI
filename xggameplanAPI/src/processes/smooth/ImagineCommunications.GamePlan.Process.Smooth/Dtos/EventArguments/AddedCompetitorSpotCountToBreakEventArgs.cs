using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class AddedCompetitorSpotCountToBreakEventArgs
        : AddedSpotToBreakEventArgs
    {
        public AddedCompetitorSpotCountToBreakEventArgs(
            BreakExternalReference breakExternalReference,
            ProductExternalReference productExternalReference,
            string productAdvertiserIdentifier,
            string productClashCode,
            SponsorshipCompetitorType competitorType,
            SponsorshipApplicability sponsorshipApplicability,
            SponsorshipCalculationType calculationType)
            : base(
                  breakExternalReference,
                  productExternalReference,
                  isSponsor: false,
                  productAdvertiserIdentifier,
                  productClashCode,
                  competitorType,
                  sponsorshipApplicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotCount, Duration.Zero))
        { }
    }
}
