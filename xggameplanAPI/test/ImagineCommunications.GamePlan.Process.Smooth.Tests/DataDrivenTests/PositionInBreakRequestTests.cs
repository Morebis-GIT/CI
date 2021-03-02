using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Position in break")]
    public class PositionInBreakRequestTests : DataDrivenTests
    {
        public PositionInBreakRequestTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName = "First in break position is respected")]
        public void PositionInBreakRequestIsRespected()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "FirstInBreakRequest_Respected"
                );

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(3, becauseArgs: null);

                _ = testResults.Recommendations.Spot("Spot_2")
                    .ShouldBePlaced()
                    .InBreak("Break_1")
                    .AtActualPositionInBreak(1)
                    .WhenRequestedPositionInBreakWas("FIB");
            }
        }

        [Fact(
            Skip = "Either there's a configuration error or a bug in the last in break code.",
            DisplayName = "Last in break position is respected"
            )]
        public void LastInBreakRequestIsRespected()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "LastInBreakRequest_Respected"
                );

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(3, becauseArgs: null);

                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InBreak("Break_1")
                    .AtActualPositionInBreak(3)
                    .WhenRequestedPositionInBreakWas("LIB");
            }
        }
    }
}
