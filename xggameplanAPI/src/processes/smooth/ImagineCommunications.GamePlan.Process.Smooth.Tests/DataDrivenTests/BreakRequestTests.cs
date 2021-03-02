using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Break requests")]
    public class BreakRequestTests : DataDrivenTests
    {
        public BreakRequestTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName = "The requested break for the spot is respected")]
        public void BreakRequest_Respected()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(BreakRequest_Respected)
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InBreak("Break_3");
            }
        }
    }
}
