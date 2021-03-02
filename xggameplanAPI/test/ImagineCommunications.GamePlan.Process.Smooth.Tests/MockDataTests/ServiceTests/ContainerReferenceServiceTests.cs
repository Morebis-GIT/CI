using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests.ServiceTests
{
    [Trait("Smooth", "Container reference service")]
    public class ContainerReferenceServiceTests
    {
        [Theory(DisplayName =
            "Given a break external reference is not in container format " +
            "When checking if it is a container reference " +
            "Then checker should be false")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("part1")]
        [InlineData("part1-part2")]
        [InlineData("part1-1-part3")]
        [InlineData("part1-part2-3")]
        [InlineData("part1-part2-part3")]
        public void IsContainerReference_BreakExternalReferenceIsContainer_ReturnsFalse(string breakExternalReference)
        {
            // Arrange

            // Act
            var res = ContainerReferenceService.IsContainerReference(breakExternalReference);

            // Assert
            _ = res.Should().BeFalse();
        }

        [Theory(DisplayName =
            "Given a break external reference is in container format " +
            "When checking it is a container reference " +
            "Then checker should be true")]
        [InlineData("break-1-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE")]
        public void IsContainerReference_BreakExternalReferenceIsContainer_ReturnsTrue(string breakExternalReference)
        {
            // Arrange

            // Act
            var res = ContainerReferenceService.IsContainerReference(breakExternalReference);

            // Assert
            _ = res.Should().BeTrue();
        }
    }
}
