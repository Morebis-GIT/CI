using System;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests.ServiceTests
{
    public partial class CanAddSpotServiceTests
    {
        private Spot SpotWithValidBreakTypeAndStartTimeFactory() =>
            _fixture
                .Build<Spot>()
                .With(p => p.BreakType, ValidBreakType)
                .With(p => p.StartDateTime, new DateTime(2020, 09, 24, 13, 30, 15))
                .With(p => p.EndDateTime, new DateTime(2020, 09, 24, 13, 30, 45))
                .Create();

        private static void MoveSpotByMinutes(Spot fakeSpot, int minutes)
        {
            fakeSpot.StartDateTime = fakeSpot.StartDateTime.AddMinutes(minutes);
            fakeSpot.EndDateTime = fakeSpot.EndDateTime.AddMinutes(minutes);
        }

        [Fact(DisplayName =
            "Given a Spot with a null BreakType " +
            "And the Spot time slot is ignored " +
            "And a Break with a valid BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot can be added to the Break"
            )]
        public void IsSpotEligible_NullSpotBreakTypeAndIgnoreSpotStartTime_SpotIsAllowed()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakType = null;

            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenIgnoringSpotTime(fakeSpot);

            // Assert
            _ = result.Should()
                .BeTrue(because: "Spots with null BreakTypes match any Break BreakType");
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time slot is ignored " +
            "But a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break"
            )]
        public void IsSpotEligible_SpotAndBreakBreakTypesMismatchesAndIgnoreSpotStartTime_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();

            Break fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.BreakType += "Different";

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenIgnoringSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: null);
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time slot is ignored " +
            "And a Break with a matching BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot can be added to the Break"
            )]
        public void IsSpotEligible_SpotAndBreakBreakTypesMatchAndIgnoreSpotStartTime_SpotIsAllowed()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenIgnoringSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeTrue(because: null);
        }

        [Fact(DisplayName =
            "Given a Spot with a null BreakType " +
            "And the Spot time slot is respected " +
            "But the Spot time slot is before the Break " +
            "And a Break with a valid BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_SpotBreakTypeIsNullAndSpotStartTimeBeforeBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakType = null;
            MoveSpotByMinutes(fakeSpot, -10);

            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: "the Break is after the allowed Spot time slot");
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time slot is respected " +
            "And the Spot time is before the break " +
            "But a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_MismatchingBreakTypeAndSpotStartTimeBeforeBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            MoveSpotByMinutes(fakeSpot, -10);

            Break fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.BreakType += "Different";

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: "the Break is after the allowed Spot time slot");
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time slot is respected " +
            "And the Spot time is before the break " +
            "And a Break with a matching BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_MatchingBreakTypeAndSpotStartTimBeforeBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            MoveSpotByMinutes(fakeSpot, -10);

            Break fakeBreak = BreakWithValidBreakTypeFactory();
            var fakeSmoothBreak = new SmoothBreak(fakeBreak, 1);

            var service = CanAddSpotService.Factory(fakeSmoothBreak);

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: "the Break is after the allowed Spot time slot");
        }

        [Fact(DisplayName =
            "Given a Spot with a null BreakType " +
            "And the Spot time is after the break " +
            "And the Spot time is respected " +
            "And a Break with a valid BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_SpotBreakTypeIsNullAndSpotStartTimeAfterBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakType = null;
            MoveSpotByMinutes(fakeSpot, 10);

            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: "the Break is before the allowed Spot time slot");
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time is after the break " +
            "And the Spot time is respected " +
            "But a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_MismatchingBreakTypeAndSpotStartTimeAfterBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            MoveSpotByMinutes(fakeSpot, 10);

            Break fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.BreakType += "Different";

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: "the Break is before the allowed Spot time slot");
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time is after the break " +
            "And the Spot time is respected " +
            "And a Break with a matching BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_MatchingBreakTypeAndSpotStartTimeAfterBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            MoveSpotByMinutes(fakeSpot, 10);

            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: null);
        }

        [Fact(DisplayName =
            "Given Spot Break Type is null " +
            "And the Spot time is inside the break " +
            "And the Spot time is respected " +
            "And a Break with a valid BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot can be added to the Break")]
        public void IsSpotEligible_SpotBreakTypeIsNullAndSpotStartTimeInsideBreak_SpotIsAllowed()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakType = null;

            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeTrue(because: null);
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time is inside the break " +
            "And the Spot time is respected " +
            "But a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break")]
        public void IsSpotEligible_MismatchingBreakTypeAndSpotStartTimeInsideBreak_SpotIsRejected()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakType += "Different";

            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeFalse(because: null);
        }

        [Fact(DisplayName =
            "Given a Spot with a valid BreakType " +
            "And the Spot time is inside the break " +
            "And the Spot time is respected " +
            "And a Break with a matching BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot can be added to the Break")]
        public void IsSpotEligible_MatchingBreakTypeAndSpotStartTimeInsideBreak_SpotIsAllowed()
        {
            // Arrange
            Spot fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            Break fakeBreak = BreakWithValidBreakTypeFactory();

            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Act
            var result = service.IsSpotEligibleWhenRespectingSpotTime(fakeSpot);

            // Assert
            _ = result.Should().BeTrue(because: null);
        }
    }
}
