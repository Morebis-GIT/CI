using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Break requests")]
    public class BreakRequestTests : DataDrivenTests
    {
        private readonly RepositoryWrapper _repositoryWrapper;

        private static Guid Spot_1Id => new Guid("00000001-0000-0000-0000-000000000000");

        public BreakRequestTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
            _repositoryWrapper = new RepositoryWrapper(RepositoryFactory);
        }

        [Theory(DisplayName = "The requested break position for the spot is respected")]
        [InlineData("1", "Break_1")]
        [InlineData("2", "Break_2")]
        [InlineData("3", "Break_3")]
        [InlineData("4", "Break_4")]
        [InlineData("Break_1", "Break_1")]
        [InlineData("Break_2", "Break_2")]
        [InlineData("Break_3", "Break_3")]
        [InlineData("Break_4", "Break_4")]
        public void BreakRequest_Respected(
            string spotBreakRequest,
            string expectedBreakReference)
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "BreakRequest"
                );

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);
            sampleSpot.BreakRequest = spotBreakRequest;
            _repositoryWrapper.SaveTestSample(sampleSpot);

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InBreak(expectedBreakReference);
            }
        }
    }
}
