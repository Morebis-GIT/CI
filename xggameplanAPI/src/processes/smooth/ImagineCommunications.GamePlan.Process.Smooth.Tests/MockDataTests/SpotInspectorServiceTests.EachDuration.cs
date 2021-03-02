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
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with advertiser failure message")]
        public void EachDurationAdvertiserComeptitorReturnsFalseAdvertiserFailureMessage()
        {
            // Act
            var result = SpotInspectorService.InspectSpot(
                _sponsoredItemEachDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(15),
                (true, "advertiser"),
                (false, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeFalse(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with no failure message")]
        public void EachDurationAdvertiserComeptitorReturnsTrueNoFailureMessage()
        {
            // Act
            var result = SpotInspectorService.InspectSpot(
                _sponsoredItemEachDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(5),
                (true, "advertiser"),
                (false, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeTrue(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with clash failure message")]
        public void EachDurationClashComeptitorReturnsFalseClashFailureMessage()
        {
            // Act
            var result = SpotInspectorService.InspectSpot(
                _sponsoredItemEachDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(15),
                (false, "advertiser"),
                (true, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeFalse(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with no failure message")]
        public void EachDurationClashComeptitorReturnsTrueClashFailureMessage()
        {
            // Act
            var result = SpotInspectorService.InspectSpot(
                _sponsoredItemEachDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(5),
                (false, "advertiser"),
                (true, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeTrue(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser and clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with both advertiser and clash " +
            "failure message")]
        public void EachDurationAdvertiserClashComeptitorReturnsFalseAdvertiserClashFailureMessage()
        {
            // Act
            var result = SpotInspectorService.InspectSpot(
                _sponsoredItemEachDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(15),
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeFalse(becauseArgs: null);
            _ = result.failureMessage.Should().HaveCount(2, becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
            _ = result.failureMessage.LastOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability each, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser and clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with both no failure message")]
        public void EachDurationAdvertiserClashComeptitorReturnsTrueNoFailureMessage()
        {
            // Act
            var result = SpotInspectorService.InspectSpot(
                _sponsoredItemEachDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(5),
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = result.isAllowed.Should().BeTrue(becauseArgs: null);
            _ = result.failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = result.failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }
    }
}
