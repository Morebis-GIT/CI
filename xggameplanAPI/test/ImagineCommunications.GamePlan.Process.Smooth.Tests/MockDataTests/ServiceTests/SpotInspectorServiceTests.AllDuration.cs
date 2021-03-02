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
        [Fact(DisplayName = "Given a sponsored item with applicability all, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with advertiser failure message")]
        public void AllDurationAdvertiserComeptitorReturnsFalseAdvertiserFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemAllDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(15),
                (true, "advertiser"),
                (false, "clash"));

            // Assert
            _ = isAllowed.Should().BeFalse(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability all, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with clash failure message")]
        public void AllDurationClashComeptitorReturnsFalseClashFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemAllDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(15),
                (false, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeFalse(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability all, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser and clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should not be allowed with both advertiser and clash " +
            "failure message")]
        public void AllDurationAdvertiserClashComeptitorReturnsFalseAdvertiserClashFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemAllDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(15),
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeFalse(becauseArgs: null);
            _ = failureMessage.Should().HaveCount(2, becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser, becauseArgs: null);
            _ = failureMessage.LastOrDefault().Should().Be(SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash, becauseArgs: null);
        }

        [Fact(DisplayName = "Given a sponsored item with applicability all, " +
            "calculation type flat, restriction type spot duration and spot is a " +
            "advertiser and clash competitor " +
            "When checking if the spot is allowed in the break " +
            "Then spot should be allowed with both no failure message")]
        public void AllDurationAdvertiserClashComeptitorReturnsTrueNoFailureMessage()
        {
            // Act
            var (isAllowed, failureMessage) = SpotInspectorService.InspectSpot(
                _sponsoredItemAllDuration,
                _restrictionAvailabilitySeven,
                "product",
                Duration.FromSeconds(5),
                (true, "advertiser"),
                (true, "clash"));

            // Assert
            _ = isAllowed.Should().BeTrue(becauseArgs: null);
            _ = failureMessage.Should().ContainSingle(becauseArgs: null);
            _ = failureMessage.FirstOrDefault().Should().Be(SmoothFailureMessages.T0_NoFailure, becauseArgs: null);
        }
    }
}
