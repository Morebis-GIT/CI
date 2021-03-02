using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Smooth only handheld spots")]
    public class SmoothHandheldSpotTests : DataDrivenTests
    {
        public SmoothHandheldSpotTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName = "Dynamic spots must not be smoothed")]
        public void SmoothWithOnlyDynamicSpots()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "SmoothHandheldOnly_DynamicOnly"
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().BeEmpty(becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Handheld spots must be smoothed")]
        public void SmoothWithOnlyHandheldSpots()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "SmoothHandheldOnly_HandheldOnly"
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(2, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Only handheld spots must be be smoothed")]
        public void SmoothWithHandheldAndDynamicSpots()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "SmoothHandheldOnly_HandheldAndDynamic"
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations[0].ExternalSpotRef.Should().Be("Spot_2", becauseArgs: null);
            }
        }
    }
}
