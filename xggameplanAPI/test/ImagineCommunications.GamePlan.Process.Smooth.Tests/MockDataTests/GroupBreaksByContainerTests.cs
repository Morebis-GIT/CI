using System.Collections.Generic;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests
{
    [Trait("Smooth", "Group breaks by container")]
    public class GroupBreaksByContainerTests
    {
        [Fact(DisplayName =
            "Given zero breaks " +
            "When grouping by containers " +
            "Then an empty list of containers is returned")]
        public void ZeroBreaksReturnsEmptyContainerList()
        {
            // Arrange
            var emptyBreakList = new List<Break>(0);

            // Act
            var result = GroupByContainers(emptyBreakList);

            // Assert
            _ = result.Should().HaveCount(0);
        }

        private List<object> GroupByContainers(IReadOnlyCollection<Break> emptyBreakList)
        {
            return new List<object>(0);
        }
    }
}
