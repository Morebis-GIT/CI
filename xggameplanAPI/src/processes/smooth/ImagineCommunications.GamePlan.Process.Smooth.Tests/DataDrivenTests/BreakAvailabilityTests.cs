using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Break availability")]
    public class BreakAvailabilityTests : DataDrivenTests
    {
        public BreakAvailabilityTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName = "Sufficient break availability is respected and the Spot is placed.")]
        public void BreakAvailability_Sufficient()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "BreakAvailability"
                );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InBreak("Break_1");
            }
        }

        [Fact(DisplayName = "Insufficient break availability is respected and the Spot is not placed.")]
        public void BreakAvailability_Insufficient()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "BreakAvailability"
                );

            using (var scope = RepositoryFactory.BeginRepositoryScope())
            {
                var breakId = new Guid("00000001-c5ec-4bc3-8aec-c24c1e3e9479");

                var breakRepository = scope.CreateRepository<IBreakRepository>();
                var breakToShorten = breakRepository.Get(breakId);

                breakToShorten.Duration = Duration.FromSeconds(12);
                breakToShorten.Avail = Duration.FromSeconds(12);
                breakToShorten.OptimizerAvail = Duration.FromSeconds(12);

                breakRepository.Remove(breakId);
                breakRepository.Add(breakToShorten);
                breakRepository.SaveChanges();
            }

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);

                testResults.Recommendations.Spot("Spot_1")
                    .ShouldBeUnplaced();
            }
        }
    }
}
