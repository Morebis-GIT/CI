using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Sponsor spots")]
    public class SponsorSpotTests : DataDrivenTests
    {
        public SponsorSpotTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName = "Given there is only one sponsored spot then the sponsor spot must go in the first break")]
        public void OneSponsorSpot()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "SponsoredSpot_OneSponsorSpot"
                );

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(2, becauseArgs: null);

                _ = testResults.Recommendations.Spot("Spot_1")
                        .ShouldBePlaced()
                        .InBreak("Break_1")
                        .AtActualPositionInBreak(1)
                        .AndShouldBeSponsored();
            }
        }

        [Fact(DisplayName = "Given there are two sponsored spots then the first spot must go in the first break and the second in the last break")]
        public void TwoSponsorSpots()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "SponsoredSpot_TwoSponsorSpots"
                );

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(2, becauseArgs: null);

                _ = testResults.Recommendations.Spot("Spot_1")
                        .ShouldBePlaced()
                        .InBreak("Break_1")
                        .AtActualPositionInBreak(1)
                        .AndShouldBeSponsored();

                _ = testResults.Recommendations.Spot("Spot_2")
                        .ShouldBePlaced()
                        .InBreak("Break_4")
                        .AtActualPositionInBreak(1)
                        .AndShouldBeSponsored();
            }
        }

        [Fact(DisplayName = "Given a prebooked spot needs to be moved in favour of a higher priority sponsor spot")]
        public void MovePreBookedSpot()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "SponsoredSpot_MovePreBookedSpot"
                );

            ReduceAvailabilityForPreBookedSpots();

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(3, becauseArgs: null);

                _ = testResults.Recommendations.Spot("Spot_1")
                        .ShouldBePlaced()
                        .ButNotBeInBreak("Break_1");
            }
        }

        private void ReduceAvailabilityForPreBookedSpots()
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            var repo = scope.CreateRepository<IBreakRepository>();

            var breakOne = repo
                .GetAll()
                .FirstOrDefault(b => b.ExternalBreakRef == "Break_1");

            if (breakOne != null)
            {
                breakOne.Avail = Duration.Zero;
                repo.SaveChanges();
            }
        }
    }
}
