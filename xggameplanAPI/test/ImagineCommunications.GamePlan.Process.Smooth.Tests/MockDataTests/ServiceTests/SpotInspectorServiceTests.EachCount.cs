using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    public partial class SpotInspectorServiceTests
    {
        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot count and spot is a " +
            "advertiser competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with advertiser failure message")]
        public void EachCountAdvertiserComeptitorReturnsFalseAdvertiserFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityZero,
                "product",
                Duration.Zero,
                (true, "advertiser"),
                (false, "clash"));

            // Assert
            _ = isAllowed.Should().BeFalse(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot count and spot is a " +
            "advertiser competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with no failure message")]
        public void EachCountAdvertiserComeptitorReturnsTrueNoFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityOne,
                "product",
                Duration.Zero,
                (true, "advertiser"),
                (false, "clash"));

            // Assert
            _ = isAllowed.Should().BeTrue(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot count and spot is a " +
            "clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with clash failure message")]
        public void EachCountClashComeptitorReturnsFalseClashFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityZero,
                "product",
                Duration.Zero,
                (false, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeFalse(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot count and spot is a " +
            "clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with no failure message")]
        public void EachCountClashComeptitorReturnsTrueClashFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityOne,
                "product",
                Duration.Zero,
                (false, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeTrue(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot count and spot is a " +
            "advertiser and clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with both advertiser and clash " +
            "failure message")]
        public void EachCountAdvertiserClashComeptitorReturnsFalseAdvertiserClashFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityZero,
                "product",
                Duration.Zero,
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeFalse(becauseArgs: null);
            _ = failureMessage.Should().HaveCount(2, becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
            _ = failureMessage.LastOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot count and spot is a " +
            "advertiser and clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with both no failure message")]
        public void EachCountAdvertiserClashComeptitorReturnsTrueNoFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemEachCount,
                _restrictionAvailabilityOne,
                "product",
                Duration.Zero,
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeTrue(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }
    }
}
