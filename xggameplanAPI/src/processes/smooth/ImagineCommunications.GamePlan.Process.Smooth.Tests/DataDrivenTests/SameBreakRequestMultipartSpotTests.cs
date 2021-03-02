using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    /// <summary>
    /// Tests for Same Break multipart spots
    /// </summary>
    [Collection("Smooth")]
    [Trait("Smooth", "Position in break")]
    public class SameBreakRequestMultipartSpotTests : DataDrivenTests
    {
        public SameBreakRequestMultipartSpotTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName = "Same break request is respected")]
        public void SameBreakRequest_Respected()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(SameBreakRequest_Respected)
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(7, becauseArgs: null);

                _ = testResults.Recommendations
                    .Where(r => r.MultipartSpot is null)
                    .Should().HaveCount(3, becauseArgs: null);

                _ = testResults.Recommendations
                    .Where(r => r.MultipartSpot == "SB")
                    .Should().HaveCount(4, becauseArgs: null);

                var topSpot = testResults.Recommendations
                    .First(r => r.MultipartSpotPosition == "TOP");

                _ = topSpot.ExternalBreakNo.Should().Be("Break_1", becauseArgs: null);
                _ = topSpot.ExternalSpotRef.Should().Be("0042", becauseArgs: null);
                _ = topSpot.MultipartSpotRef.Should().Be("0043,0044,0045", becauseArgs: null);

                foreach (var spotToCheck in topSpot
                    .MultipartSpotRef
                    .Split(new[] { ',' }))
                {
                    _ = testResults.Recommendations.First(r => r.ExternalSpotRef == spotToCheck)
                        .ExternalBreakNo.Should().NotBeNullOrWhiteSpace(becauseArgs: null);
                }
            }
        }
    }
}
