using System;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    public class AddedSpotToBreakEventArgs
        : EventArgs
    {
        /// <summary>
        /// Construct an event argument object for when a spot is added to a break.
        /// </summary>
        /// <param name="productExternalReference">   </param>
        /// <param name="isSponsor">                  </param>
        /// <param name="productAdvertiserIdentifier"></param>
        /// <param name="productClashCode">           </param>
        /// <param name="competitorType">             </param>
        /// <param name="sponsorshipApplicability">   
        /// Will be <see langword="null"/> if the spot is for a sponsor.
        /// </param>
        /// <param name="restrictionType">            </param>
        protected AddedSpotToBreakEventArgs(
            BreakExternalReference breakExternalReference,
            ProductExternalReference productExternalReference,
            bool isSponsor,
            string productAdvertiserIdentifier,
            string productClashCode,
            SponsorshipCompetitorType competitorType,
            SponsorshipApplicability sponsorshipApplicability,
            SponsorshipCalculationType calculationType,
            (SponsorshipRestrictionType type, Duration duration) restrictionType)
        {
            BreakExternalReference = breakExternalReference;
            ProductExternalReference = productExternalReference;
            IsSponsor = isSponsor;
            ProductAdvertiserIdentifier = productAdvertiserIdentifier;
            CompetitorType = competitorType;
            ProductClashCode = productClashCode;
            CalculationType = calculationType;
            Applicability = sponsorshipApplicability;
            RestrictionType = restrictionType;
            if (!isSponsor)
            {
                if (CompetitorType == SponsorshipCompetitorType.Neither)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(CompetitorType),
                        "Cannot have competitor type Neither for competitor spots."
                        );
                }
            }

            if (String.IsNullOrWhiteSpace(productExternalReference))
            {
                throw new ArgumentNullException(nameof(productExternalReference));
            }
        }

        public BreakExternalReference BreakExternalReference { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }
        public SponsorshipCompetitorType CompetitorType { get; }
        public bool IsSponsor { get; }
        public string ProductAdvertiserIdentifier { get; }
        public string ProductClashCode { get; }
        public ProductExternalReference ProductExternalReference { get; }
        public (SponsorshipRestrictionType type, Duration duration) RestrictionType { get; }
        public SponsorshipApplicability Applicability { get; }
    }
}
