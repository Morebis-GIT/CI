using System;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class RemovedSpotFromBreakEventArgs
        : EventArgs
    {
        protected RemovedSpotFromBreakEventArgs(
            BreakExternalReference breakExternalReference,
            string advertiserIdentifier,
            string clashExternalReference,
            ProductExternalReference productExternalReference,
            bool isSponsor,
            SponsorshipApplicability applicability,
            SponsorshipCalculationType calculationType,
            (SponsorshipRestrictionType type, Duration duration) restrictionType
)
        {
            IsSponsor = isSponsor;
            ProductExternalReference = productExternalReference;
            Applicability = applicability;
            CalculationType = calculationType;
            RestrictionType = restrictionType;
            BreakExternalReference = breakExternalReference;
            ProductAdvertiserIdentifier = advertiserIdentifier;
            ProductClashCode = clashExternalReference;

            if (String.IsNullOrWhiteSpace(productExternalReference))
            {
                throw new ArgumentNullException(nameof(productExternalReference));
            }
        }

        public SponsorshipApplicability Applicability { get; set; }
        public BreakExternalReference BreakExternalReference { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }
        public bool IsSponsor { get; set; }
        public string ProductAdvertiserIdentifier { get; }
        public string ProductClashCode { get; }
        public ProductExternalReference ProductExternalReference { get; }
        public (SponsorshipRestrictionType Type, Duration Duration) RestrictionType { get; }
    }
}
