using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests.ServiceTests
{
    public partial class CanAddSpotServiceTests
    {
        [Theory(DisplayName =
            "Given a smooth break with a non-container reference format has been provided" +
            "When creating the can add spot service " +
            "Then the standard can add spot service is returned"
            )]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("part1")]
        [InlineData("part1-part2")]
        [InlineData("part1-1-part3")]
        [InlineData("part1-part2-3")]
        [InlineData("part1-part2-part3")]
        public void Factory_PassesInBreakExternalReference_ReturnsCanAddSpotService(string breakExternalReference)
        {
            // Arrange
            Break fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.ExternalBreakRef = breakExternalReference;

            // Act
            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Assert
            _ = service.Should()
                .BeOfType<CanAddSpotService>(string.Empty, becauseArgs: null);
        }

        [Theory(DisplayName =
            "Given a smooth break with a non-container reference format has been provided" +
            "When creating the can add spot service " +
            "Then the standard can add spot service is returned"
            )]
        [InlineData("break-1-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE")]
        public void Factory_PassesInContainerReference_ReturnsCanAddSpotToContainerService(string breakExternalReference)
        {
            // Arrange
            Break fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.ExternalBreakRef = breakExternalReference;

            // Act
            var service = CanAddSpotService.Factory(new SmoothBreak(fakeBreak, 1));

            // Assert
            _ = service.Should()
                .BeOfType<CanAddSpotToContainerService>(string.Empty, becauseArgs: null);
        }
    }
}
