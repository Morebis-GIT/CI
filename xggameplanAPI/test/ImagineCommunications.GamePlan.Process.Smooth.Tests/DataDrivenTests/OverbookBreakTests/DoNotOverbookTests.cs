using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Overbooking tests")]
    public class DoNotOverbookTests : DataDrivenTests
    {
        private static Guid Break_1Id => new Guid("00000001-0000-0000-0000-000000000000");

        private readonly RepositoryWrapper _repositoryWrapper;

        public DoNotOverbookTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
            _repositoryWrapper = new RepositoryWrapper(RepositoryFactory);
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

            var testBreak = _repositoryWrapper.LoadTestBreak(Break_1Id);
            testBreak.Avail = Duration.Zero;
            testBreak.OptimizerAvail = Duration.Zero;
            _repositoryWrapper.SaveTestSample(testBreak);

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);

                testResults.Recommendations.Spot("Spot_2")
                    .ShouldBeUnplaced();

                var theBreak = _repositoryWrapper.LoadTestBreak(Break_1Id);

                _ = theBreak.Avail.Should()
                    .Be(Duration.Zero, becauseArgs: null);

                _ = testResults.SmoothFailures[0].ExternalSpotRef.Should()
                    .Be("Spot_2", null);

                _ = testResults.SmoothFailures[0].MessageIds[0].Should()
                    .Be((int)SmoothFailureMessages.T1_InsufficentRemainingDuration, null);
            }
        }
    }
}
