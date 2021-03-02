using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Break availability")]
    public class BreakAvailabilityTests : DataDrivenTests
    {
        private const string BreakOne = "Break_1";
        private const string SpotOne = "Spot_1";

        private readonly RepositoryWrapper _repositoryWrapper;

        public BreakAvailabilityTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
            _repositoryWrapper = new RepositoryWrapper(RepositoryFactory);
        }

        [Fact(DisplayName = "Sufficient break availability is respected and the Spot is placed.")]
        public void BreakAvailability_Sufficient()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(BreakAvailabilityTests)
                );

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations.Spot(SpotOne)
                    .ShouldBePlaced()
                    .InBreak(BreakOne);
            }
        }

        [Fact(DisplayName = "Insufficient break availability is respected and the Spot is not placed.")]
        public void BreakAvailability_Insufficient()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(BreakAvailabilityTests)
                );

            var allBreaks = _repositoryWrapper.LoadAllTestBreaks();
            var breakToShorten = allBreaks.Single(b => b.ExternalBreakRef == BreakOne);

            breakToShorten.Duration = Duration.FromSeconds(12);
            breakToShorten.Avail = Duration.FromSeconds(12);
            breakToShorten.OptimizerAvail = Duration.FromSeconds(12);

            _repositoryWrapper.SaveTestSample(breakToShorten);

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);

                testResults.Recommendations.Spot(SpotOne)
                    .ShouldBeUnplaced();
            }
        }
    }
}
