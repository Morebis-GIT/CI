using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class SpotInspectorService
    {
        private static readonly (bool, IEnumerable<SmoothFailureMessages>)
            _spotIsAllowed = (true, new[] { SmoothFailureMessages.T0_NoFailure });

        /// <summary>
        /// <para>
        /// This method can be used by the restriction checking service to
        /// determine if a spot can be added to a break. The method must ONLY
        /// return one of the following:
        /// </para>
        /// <para>
        /// <list type="number">
        /// <item>
        /// <description>
        /// the boolean <c>false</c> and a SmoothFailure if the spot cannot be
        /// added due to a restriction already being at the maximum allowed;
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// the boolean <c>true</c> and SmoothFailureMessages.T0_NoFailure when
        /// the spot is not restricted.
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        ///</summary>
        public static (bool isAllowed, IEnumerable<SmoothFailureMessages> failureMessage)
            InspectSpot(
            SponsoredItem sponsoredItem,
            SmoothSponsorshipRestrictionLimits restrictionsAvailability,
            ProductExternalReference spotProductExternalReference,
            Duration spotLength,
            (bool spotIsACompetitorByAdvertiser, string advertiserIdentifier) advertiser,
            (bool spotIsACompetitorByClashCode, string clashCode) clash)
        {
            ExamineSponsorship(sponsoredItem, restrictionsAvailability);

            if (!(advertiser.spotIsACompetitorByAdvertiser ||
                  clash.spotIsACompetitorByClashCode))
            {
                return _spotIsAllowed;
            }

            double remainingAvailability = FindRemainingAvailability(
                restrictionsAvailability,
                spotProductExternalReference,
                advertiser,
                clash);

            if (remainingAvailability == 0)
            {
                return (false, GenerateFailureMessages(advertiser, clash));
            }

            IEnumerable<SmoothFailureMessages> failureMessages =
                DissectSpot(
                    sponsoredItem,
                    remainingAvailability,
                    spotLength,
                    advertiser,
                    clash);

            if (failureMessages.Any())
            {
                return (false, failureMessages);
            }
            else
            {
                return _spotIsAllowed;
            }
        }

        private static IEnumerable<SmoothFailureMessages> DissectSpot(
            SponsoredItem sponsoredItem,
            double remainingAvailabilityForProduct,
            Duration spotLength,
            (bool spotIsACompetitorByAdvertiser, string advertiserIdentifier) advertiser,
            (bool spotIsACompetitorByClashCode, string clashCode) clash)
        {
            IEnumerable<SmoothFailureMessages> failureMessages =
                new List<SmoothFailureMessages>();
            SponsorshipRestrictionType restrictionType =
                GetRestrictionTypeAndValue(
                    sponsoredItem,
                    advertiser,
                    clash);

            if (NoAvailabilityForProduct())
            {
                failureMessages = GenerateFailureMessages(advertiser, clash);
            }

            return failureMessages;

            bool NoAvailabilityForProduct()
            {
                if (restrictionType == SponsorshipRestrictionType.SpotDuration)
                {
                    return spotLength.TotalSeconds > remainingAvailabilityForProduct;
                }
                else
                {
                    return 1 > remainingAvailabilityForProduct;
                }
            }
        }

        private static void ExamineSponsorship(
              SponsoredItem sponsoredItem,
              SmoothSponsorshipRestrictionLimits restrictionsAvailability)
        {
            if (sponsoredItem is null)
            {
                throw new ArgumentNullException(nameof(sponsoredItem));
            }

            if (restrictionsAvailability is null)
            {
                throw new ArgumentNullException(nameof(restrictionsAvailability));
            }

            if (!sponsoredItem.Applicability.HasValue)
            {
                throw new ArgumentNullException(nameof(sponsoredItem.Applicability));
            }
        }

        private static double FindRemainingAvailability(
            SmoothSponsorshipRestrictionLimits restrictionsAvailability,
            ProductExternalReference spotProductExternalReference,
            (bool spotIsACompetitorByAdvertiser, string advertiserIdentifier) advertiser,
            (bool spotIsACompetitorByClashCode, string clashCode) clash)
        {
            if (restrictionsAvailability.AvailabilitiesForCompetitors.Count == 0)
            {
                return 0;
            }
            if (restrictionsAvailability.AvailabilitiesForCompetitors
                 .TryGetValue(
                 spotProductExternalReference,
                 out double remainingAvailabilityForProduct))
            {
                return remainingAvailabilityForProduct;
            }

            double remainingAvailabilityForAdvertiser = double.MaxValue;
            double remainingAvailabilityForClash = double.MaxValue;
            if (advertiser.spotIsACompetitorByAdvertiser)
            {
                if (!restrictionsAvailability.AvailabilitiesForCompetitors
                .TryGetValue(
                advertiser.advertiserIdentifier,
                out remainingAvailabilityForAdvertiser))
                {
                    remainingAvailabilityForAdvertiser = 0;
                }
            }
            if (clash.spotIsACompetitorByClashCode)
            {
                if (!restrictionsAvailability.AvailabilitiesForCompetitors
                .TryGetValue(
                clash.clashCode,
                out remainingAvailabilityForClash))
                {
                    remainingAvailabilityForClash = 0;
                }
            }

            return (remainingAvailabilityForAdvertiser < remainingAvailabilityForClash) ?
                remainingAvailabilityForAdvertiser :
                remainingAvailabilityForClash;
        }

        private static IEnumerable<SmoothFailureMessages> GenerateFailureMessages(
            (bool spotIsACompetitorByAdvertiser, string advertiserIdentifier) advertiser,
            (bool spotIsACompetitorByClashCode, string clashCode) clash)
        {
            var failureMessages = new List<SmoothFailureMessages>();
            if (advertiser.spotIsACompetitorByAdvertiser)
            {
                failureMessages.Add(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser);
            }
            if (clash.spotIsACompetitorByClashCode)
            {
                failureMessages.Add(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash);
            }

            return failureMessages;
        }

        private static SponsorshipRestrictionType GetRestrictionTypeAndValue(
            SponsoredItem sponsoredItem,
            (bool spotIsACompetitorByAdvertiser, string advertiserIdentifier) advertiser,
            (bool spotIsACompetitorByClashCode, string clashCode) clash)
        {
            if (sponsoredItem.Applicability == SponsorshipApplicability.EachCompetitor)
            {
                SponsorshipRestrictionType restrictionType =
                    SponsorshipRestrictionType.SpotCount;

                if (advertiser.spotIsACompetitorByAdvertiser)
                {
                    var advertiserExclusivity =
                        sponsoredItem.AdvertiserExclusivities.Find(a =>
                        a.AdvertiserIdentifier.Equals(advertiser.advertiserIdentifier));

                    if (!advertiserExclusivity.RestrictionType.HasValue)
                    {
                        throw new ArgumentNullException(nameof(advertiserExclusivity.RestrictionType));
                    }

                    restrictionType = advertiserExclusivity.RestrictionType.Value;
                }
                if (clash.spotIsACompetitorByClashCode)
                {
                    var clashExclusivity = sponsoredItem.ClashExclusivities.Find(a =>
                        a.ClashExternalRef.Equals(clash.clashCode));

                    if (!clashExclusivity.RestrictionType.HasValue)
                    {
                        throw new ArgumentNullException(nameof(clashExclusivity.RestrictionType));
                    }

                    restrictionType = clashExclusivity.RestrictionType.Value;
                }

                return restrictionType;
            }
            else
            {
                if (!sponsoredItem.RestrictionType.HasValue)
                {
                    throw new ArgumentNullException(nameof(sponsoredItem.RestrictionType));
                }

                return sponsoredItem.RestrictionType.Value;
            }
        }
    }
}
