using System;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Spot and Break BreakType tests")]
    public class SpotAndBreakComparisonTests
         : DataDrivenTests
    {
        private static Guid Break_1Id => new Guid("00000001-0000-0000-0000-000000000000");
        private static Guid Spot_1Id => new Guid("00000001-ab2c-455c-b793-00161d859d36");

        private readonly RepositoryWrapper _repositoryWrapper;

        public SpotAndBreakComparisonTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
            _repositoryWrapper = new RepositoryWrapper(RepositoryFactory);
        }

        [Fact(DisplayName =
            "Given a Spot with a null BreakType " +
            "When checking the Spot against a Break " +
            "Then the Spot can be added to a Break with any BreakType"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeIsNull_SpotCanBeAddedToTheBreak()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(SpotAndBreakComparisonTests)
                );

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);
            sampleSpot.BreakType = null;
            _repositoryWrapper.SaveTestSample(sampleSpot);

            // Act
            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InBreak("Break_1");
            }
        }

        [Fact(DisplayName =
            "Given a Spot with a BreakType " +
            "And a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to the Break"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeMismatchBreakBreakType_SpotCannotBeAddedToTheBreak()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(SpotAndBreakComparisonTests)
                );

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);
            sampleSpot.BreakType = "PR-PREMIUM";
            _repositoryWrapper.SaveTestSample(sampleSpot);

            var sampleBreak = _repositoryWrapper.LoadTestBreak(Break_1Id);
            sampleBreak.BreakType = "WE-WEATHERUPDATE";
            _repositoryWrapper.SaveTestSample(sampleBreak);

            // Act
            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                testResults.Recommendations.Spot("Spot_1").ShouldBeUnplaced();
            }
        }

        [Fact(DisplayName =
            "Given a Spot with a BreakType " +
            "And a Break with a mismatched BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot cannot be added to Break" +
            "And the SmoothFailure is BreakType mismatch"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeMismatchBreakBreakType_SmoothFailureIsBreakTypeMismatch()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(SpotAndBreakComparisonTests)
                );

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);
            sampleSpot.BreakType = "PR-PREMIUM";
            _repositoryWrapper.SaveTestSample(sampleSpot);

            // Act
            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                testResults.Recommendations.Spot("Spot_1").ShouldBeUnplaced();

                var spotOneFailures = testResults.SmoothFailures.Where(x => x.ExternalSpotRef == "Spot_1");

                _ = spotOneFailures.Should().HaveCount(4, because: "the BreakType does not match any of the breaks");
                _ = spotOneFailures.All(x => x.MessageIds.Contains((int)SmoothFailureMessages.T1_InvalidBreakType));
            }
        }

        [Fact(DisplayName =
            "Given a Spot with a BreakType " +
            "And a Break with a matching BreakType " +
            "When checking the Spot against the Break " +
            "Then the Spot can be added to the Break"
            )]
        public void
        CanAddSpotWithBreakType_SpotBreakTypeMatchesBreaksBreakType_SpotCanBeAddedToTheBreak()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                nameof(SpotAndBreakComparisonTests)
                );

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);
            sampleSpot.BreakType = "PR-PREMIUM";
            _repositoryWrapper.SaveTestSample(sampleSpot);

            var sampleBreak = _repositoryWrapper.LoadTestBreak(Break_1Id);
            sampleBreak.BreakType = "PR-PREMIUM";
            _repositoryWrapper.SaveTestSample(sampleBreak);

            // Act
            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InBreak("Break_1");
            }
        }
    }
}
