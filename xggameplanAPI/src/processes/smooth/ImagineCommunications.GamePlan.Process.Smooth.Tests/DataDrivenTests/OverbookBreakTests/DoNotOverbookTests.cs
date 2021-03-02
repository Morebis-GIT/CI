using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Overbooking tests")]
    public class DoNotOverbookTests : DataDrivenTests
    {
        public DoNotOverbookTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        [Fact(DisplayName =
            "Given a break full of dynamic spots " +
            "When trying to place a client picked spot " +
            "Then there is no room in the break so the spot is not placed."
            )]
        public void DoNotOverbookBreak()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(DoNotOverbookBreak)
                );

            var testBreak = GetTestBreak(Break_1Id);
            testBreak.Avail = Duration.Zero;
            testBreak.OptimizerAvail = Duration.Zero;
            SaveTestBreak(testBreak);

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);

                testResults.Recommendations.Spot("Spot_2")
                    .ShouldBeUnplaced();

                var theBreak = GetTestBreak(Break_1Id);

                _ = theBreak.Avail.Should()
                    .Be(Duration.Zero, becauseArgs: null);

                _ = testResults.SmoothFailures[0].ExternalSpotRef.Should()
                    .Be("Spot_2", null);

                _ = testResults.SmoothFailures[0].MessageIds[0].Should()
                    .Be((int)SmoothFailureMessages.T1_InsufficentRemainingDuration, null);
            }
        }

        private static Guid Break_1Id => new Guid("00000001-c5ec-4bc3-8aec-c24c1e3e9479");

        private Break GetTestBreak(Guid breakId)
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            var breakRepository = scope.CreateRepository<IBreakRepository>();

            return breakRepository.Get(breakId);
        }

        private void SaveTestBreak(Break aBreak)
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            var breakRepository = scope.CreateRepository<IBreakRepository>();

            breakRepository.Add(aBreak);
            breakRepository.SaveChanges();
        }
    }
}
