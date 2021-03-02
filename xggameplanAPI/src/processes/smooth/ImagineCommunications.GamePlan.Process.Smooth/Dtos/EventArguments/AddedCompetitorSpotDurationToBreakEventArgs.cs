using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class AddedCompetitorSpotDurationToBreakEventArgs
        : AddedSpotToBreakEventArgs
    {
        public AddedCompetitorSpotDurationToBreakEventArgs(
            BreakExternalReference breakExternalReference,
            ProductExternalReference productExternalReference,
            string productAdvertiserIdentifier,
            string productClashCode,
            SponsorshipCompetitorType competitorType,
            SponsorshipApplicability sponsorshipApplicability,
            SponsorshipCalculationType calculationType,
            Duration spotLength)
            : base(
                  breakExternalReference,
                  productExternalReference,
                  isSponsor: false,
                  productAdvertiserIdentifier,
                  productClashCode,
                  competitorType,
                  sponsorshipApplicability,
                  calculationType,
                  (SponsorshipRestrictionType.SpotDuration, spotLength))
        { }
    }
}
