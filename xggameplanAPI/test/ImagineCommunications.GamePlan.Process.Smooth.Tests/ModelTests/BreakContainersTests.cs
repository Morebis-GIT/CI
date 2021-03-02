using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.ModelTests
{
    [Trait("Smooth", "Break containers")]
    public class BreakContainerTests
    {
        private readonly SafeFixture _fixture;

        public BreakContainerTests() => _fixture = new SafeFixture();

        [Fact(DisplayName =
            "Given no breaks " +
            "When grouping breaks in to containers " +
            "Then the breaks container is empty"
            )]
        public void EmptyBreakListProduceEmptyBreakContainer()
        {
            // Arrange
            var noBreaks = new List<Break>(0);

            // Act
            var result = BreakContainers.GroupBreaks(noBreaks);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Fact(DisplayName =
            "Given breaks not in containers " +
            "When grouping breaks in to containers " +
            "Then the breaks container is empty"
            )]
        public void BreaksNotInContainersProduceEmptyBreakContainer()
        {
            // Arrange
            var breaks = _fixture
                .CreateMany<Break>(3)
                .ToList();

            for (int idx = 1; idx <= breaks.Count; idx++)
            {
                breaks[idx - 1].ExternalBreakRef = "BreakRef-" + idx.ToString();
            }

            // Act
            var result = BreakContainers.GroupBreaks(breaks);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Fact(DisplayName =
            "Given 3 breaks marked as within 2 containers " +
            "But one of the breaks is not a container break " +
            "When grouping breaks in to containers " +
            "Then the break container is empty.")]
        public void BreaksWithMixOfContainersAndNonContainersProducesEmptyBreakContainer()
        {
            // Arrange
            var breaks = _fixture
                .CreateMany<Break>(3)
                .ToList();

            breaks[0].ExternalBreakRef = "BreakRef-1-1";
            breaks[1].ExternalBreakRef = "BreakRef-1";
            breaks[2].ExternalBreakRef = "BreakRef-2-1";

            // Act
            var result = BreakContainers.GroupBreaks(breaks);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Fact(DisplayName =
            "Given 3 breaks marked as within 2 containers " +
            "When grouping breaks in to containers " +
            "Then the break container has two containers.")]
        public void BreaksWithTwoContainersGroupIntoTwoContainers()
        {
            // Arrange
            var breaks = _fixture
                .CreateMany<Break>(3)
                .ToList();

            breaks[0].ExternalBreakRef = "BreakRef-1-1";
            breaks[1].ExternalBreakRef = "BreakRef-1-2";
            breaks[2].ExternalBreakRef = "BreakRef-2-1";

            // Act
            var result = BreakContainers.GroupBreaks(breaks);

            // Assert
            _ = result.Should().HaveCount(2);
        }

        [Fact(DisplayName =
            "Given 3 breaks marked as within 2 containers " +
            "When grouping breaks in to containers " +
            "Then the breaks must be in the correct container.")]
        public void BreaksWithTwoContainersGroupIntoTwoContainersWhereEachBreakIsInTheCorrectContainer()
        {
            // Arrange
            var breaks = _fixture
                .CreateMany<Break>(3)
                .ToList();

            breaks[0].ExternalBreakRef = "BreakRef-1-1";
            breaks[1].ExternalBreakRef = "BreakRef-1-2";
            breaks[2].ExternalBreakRef = "BreakRef-2-1";

            // Act
            var result = BreakContainers.GroupBreaks(breaks);

            // Assert
            var confirmThatContainerOne = result["BreakRef-1-1"];
            _ = confirmThatContainerOne.Should().HaveCount(2);
            _ = confirmThatContainerOne[0].Should().BeSameAs(breaks[0]);
            _ = confirmThatContainerOne[1].Should().BeSameAs(breaks[1]);

            var confirmThatContainerTwo = result["BreakRef-2-1"];
            _ = confirmThatContainerTwo.Should().HaveCount(1);
            _ = confirmThatContainerTwo[0].Should().BeSameAs(breaks[2]);
        }
    }
}
