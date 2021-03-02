using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Spot inspector service")]
    public partial class SpotInspectorServiceTests
    {
        private static readonly SmoothSponsorshipRestrictionLimits
            _restrictionAvailabilityZero = CreateRestrictionLimits(0);

        private static readonly SmoothSponsorshipRestrictionLimits
            _restrictionAvailabilityOne = CreateRestrictionLimits(1);

        private static readonly SmoothSponsorshipRestrictionLimits
            _restrictionAvailabilitySeven = CreateRestrictionLimits(7);

        private static readonly SponsoredItem _sponsoredItemAllCount =
            CreateSponsoredItemApplicabiltiyAll(
                SponsorshipCalculationType.Flat,
                SponsorshipRestrictionType.SpotCount,
                5);

        private static readonly SponsoredItem _sponsoredItemEachCount =
            CreateSponsoredItemApplicabiltiyEach(
                SponsorshipCalculationType.Flat,
                SponsorshipRestrictionType.SpotCount,
                5,
                7);

        private static readonly SponsoredItem _sponsoredItemAllDuration =
            CreateSponsoredItemApplicabiltiyAll(
                SponsorshipCalculationType.Flat,
                SponsorshipRestrictionType.SpotDuration,
                5);

        private static readonly SponsoredItem _sponsoredItemEachDuration =
            CreateSponsoredItemApplicabiltiyEach(
                SponsorshipCalculationType.Flat,
                SponsorshipRestrictionType.SpotDuration,
                5,
                7);

        [Fact(DisplayName = "Given sponsored item is null " +
            "When checking if the spot is allowed in the break " +
            "Then exception should be thrown")]
        public void NullSponsoredItemShouldThrowNull()
        {
            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
              {
                  // Act
                  _ = SpotInspectorService.InspectSpot(
                      null,
                      _restrictionAvailabilitySeven,
                      string.Empty,
                      Duration.Zero,
                      (false, string.Empty),
                      (false, string.Empty));
              });
        }

        [Fact(DisplayName = "Given restriction limits is null " +
            "When checking if the spot is allowed in the break " +
            "Then exception should be thrown")]
        public void NullRestrictionLimitsShouldThrowNull()
        {
            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
              {
                  // Act
                  _ = SpotInspectorService.InspectSpot(
                      _sponsoredItemAllCount,
                      null,
                      string.Empty,
                      Duration.Zero,
                      (false, string.Empty),
                      (false, string.Empty));
              });
        }

        [Fact(DisplayName = "Given the spot is a non-competitor " +
            "When checking if the spot is allowed in the break " +
            "Then should return true and no failure message")]
        public void NotACompetitorSpotReturnsTrueAndNoFailure()
        {
            // Act
            var result =
                SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityOne,
                string.Empty,
                Duration.Zero,
                (false, string.Empty),
                (false, string.Empty));

            // Assert
            _ = result.isAllowed.Should().BeTrue(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }

        [Fact(DisplayName = "Given the spot is a-competitor and the product " +
            "external reference is not found in the restriction " +
            "When checking if the spot is allowed in the break " +
            "Then should return true and no failure message")]
        public void NoProductFoundReturnsTrueAndNoFailure()
        {
            // Act
            var result =
                SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityOne,
                string.Empty,
                Duration.Zero,
                (true, "unavailable advertiser"),
                (true, "unavailable clash"));

            // Assert
            _ = result.isAllowed.Should().BeFalse(becauseArgs: null);
            _ = result.failureMessage.Should().HaveCount(2, becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
            _ = result.failureMessage.LastOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given the spot is a-competitor and the competitor product " +
            "is not found within the restriction availability " +
            "When checking if the spot is allowed in the break " +
            "Then should use the advertiser and clash availability")]
        public void NoCompetitorProductFoundInRestrictionShouldUseAdvertiserAndClash()
        {
            // Act
            var result =
                SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityOne,
                string.Empty,
                Duration.Zero,
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeTrue(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }

        [Fact(DisplayName = "Given sponsored item applicability is null " +
            "When checking if the spot is allowed in the break " +
            "Then exception should be thrown")]
        public void NullSponsoredItemApplicabilityThowsNull()
        {
            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _ = SpotInspectorService.InspectSpot(
                    new SponsoredItem(),
                    _restrictionAvailabilityZero,
                    string.Empty,
                    Duration.Zero,
                    (true, string.Empty),
                    (true, string.Empty));
            });
        }

        private static SponsoredItem CreateSponsoredItemApplicabiltiyAll(
            SponsorshipCalculationType calculationType,
            SponsorshipRestrictionType restrictionType,
            int restrictionValue)
        {
            return new SponsoredItem()
            {
                Applicability = SponsorshipApplicability.AllCompetitors,
                AdvertiserExclusivities = new List<AdvertiserExclusivity>()
                {
                    new AdvertiserExclusivity()
                    {
                        AdvertiserIdentifier = "advertiser"
                    }
                },
                ClashExclusivities = new List<ClashExclusivity>()
                {
                    new ClashExclusivity()
                    {
                        ClashExternalRef = "clash"
                    }
                },
                Products = null,
                RestrictionType = restrictionType,
                RestrictionValue = restrictionValue,
                CalculationType = calculationType,
                SponsorshipItems = null
            };
        }

        private static SponsoredItem CreateSponsoredItemApplicabiltiyEach(
            SponsorshipCalculationType calculationType,
            SponsorshipRestrictionType restrictionType,
            int advertiserRestrictionValue,
            int clashRestrictionValue)
        {
            return new SponsoredItem()
            {
                Applicability = SponsorshipApplicability.EachCompetitor,
                AdvertiserExclusivities = new List<AdvertiserExclusivity>()
                {
                    new AdvertiserExclusivity()
                    {
                        AdvertiserIdentifier = "advertiser",
                        RestrictionType = restrictionType,
                        RestrictionValue = advertiserRestrictionValue
                    }
                },
                ClashExclusivities = new List<ClashExclusivity>()
                {
                    new ClashExclusivity()
                    {
                        ClashExternalRef = "clash",
                        RestrictionType = restrictionType,
                        RestrictionValue = clashRestrictionValue
                    }
                },
                Products = null,
                RestrictionType = null,
                RestrictionValue = null,
                CalculationType = calculationType,
                SponsorshipItems = null
            };
        }

        private static SmoothSponsorshipRestrictionLimits CreateRestrictionLimits(int productAvailability) =>
            new SmoothSponsorshipRestrictionLimits()
            {
                AvailabilitiesForCompetitors = new Dictionary<ProductExternalReference, double>()
                {
                    ["product"] = productAvailability,
                    ["advertiser"] = productAvailability,
                    ["clash"] = productAvailability
                }
            };
    }
}
