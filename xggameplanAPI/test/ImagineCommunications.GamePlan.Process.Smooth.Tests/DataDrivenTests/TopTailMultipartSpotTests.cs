using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    /// <summary>
    /// Tests for Top/Tail multipart spots
    /// </summary>
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Multipart spots")]
    public class TopTailMultipartSpotTests : DataDrivenTests
    {
        public TopTailMultipartSpotTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        /// <summary>
        /// Test that TT request respected
        /// </summary>
        [Fact(DisplayName = "Top Tail requests are respected")]
        public void TopTailRequest_Respected()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(TopTailRequest_Respected)
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                const string breakExternalReference = "Break_1";

                _ = testResults.Recommendations.Should().HaveCount(3, becauseArgs: null);

                _ = testResults.Recommendations
                    .Where(r => r.MultipartSpot == "TT")
                    .Should().HaveCount(2, becauseArgs: null);

                var topSpot = testResults.Recommendations
                    .First(r => r.MultipartSpotPosition == "TOP");

                _ = topSpot.ExternalSpotRef.Should().Be("Spot_1", becauseArgs: null);
                _ = topSpot.MultipartSpotRef.Should().Be("Spot_2", becauseArgs: null);
                _ = topSpot.ExternalBreakNo.Should().Be(breakExternalReference, becauseArgs: null);

                var tailSpot = testResults.Recommendations
                    .First(r => r.MultipartSpotPosition == "TAIL");

                _ = tailSpot.ExternalSpotRef.Should().Be("Spot_2", becauseArgs: null);
                _ = tailSpot.MultipartSpotRef.Should().Be("Spot_1", becauseArgs: null);
                _ = tailSpot.ExternalBreakNo.Should().Be(breakExternalReference, becauseArgs: null);
            }
        }
    }
}
