using System;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using NodaTime;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests.ServiceTests
{
    [Trait("Smooth", "CanAddSpot service")]
    public partial class CanAddSpotServiceTests
    {
        private const string ValidBreakType = "VA-VALIDBREAKTYPE";
        private readonly Fixture _fixture = new SafeFixture();

        private Break BreakWithValidBreakTypeFactory() =>
            _fixture
                .Build<Break>()
                .With(p => p.BreakType, ValidBreakType)
                .With(p => p.ScheduledDate, new DateTime(2020, 09, 24, 13, 30, 0))
                .With(p => p.Duration, Duration.FromMinutes(2))
                .Create();

        [Fact(DisplayName =
            "Given a Spot with a null BreakType " +
            "When checking the Spot against a Break " +
            "Then the Spot can be added to a Break with any BreakType"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeIsNull_SpotCanBeAddedToTheBreak()
        {
            // Arrange
            Break fakeBreak = BreakWithValidBreakTypeFactory();
            var fakeSmoothBreak = new SmoothBreak(fakeBreak, 1);

            var service = new CanAddSpotService(fakeSmoothBreak);

            // Act
            var result = service.CanAddSpotWithBreakType(null);

            // Assert
            _ = result.Should().BeTrue(because: null);
        }

        [Fact(DisplayName =
            "Given a Spot with a BreakType " +
            "And a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to Break"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeMismatchBreakBreakType_SpotCannotBeAddedToTheBreak()
        {
            // Arrange
            Break fakeBreak = BreakWithValidBreakTypeFactory();
            var fakeSmoothBreak = new SmoothBreak(fakeBreak, 1);

            var service = new CanAddSpotService(fakeSmoothBreak);

            // Act
            var result = service.CanAddSpotWithBreakType("DF-DIFFERENT BREAK TYPE");

            // Assert
            _ = result.Should().BeFalse(because: null);
        }

        [Fact(DisplayName =
            "Given a Spot with a BreakType " +
            "And a Break with a matching BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot can be added to Break"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeMatchesBreaksBreakType_SpotCanBeAddedToTheBreak()
        {
            // Arrange
            Break fakeBreak = BreakWithValidBreakTypeFactory();
            var fakeSmoothBreak = new SmoothBreak(fakeBreak, 1);

            var service = new CanAddSpotService(fakeSmoothBreak);

            // Act
            var result = service.CanAddSpotWithBreakType(ValidBreakType);

            // Assert
            _ = result.Should().BeTrue(because: null);
        }
    }
}
