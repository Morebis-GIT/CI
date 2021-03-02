using System.IO;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "XG10/11 Sponsorship - Unplaced Pass")]
    public class DoNotPlaceCompetitorsInPass7001Tests : DataDrivenTests
    {
        public DoNotPlaceCompetitorsInPass7001Tests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, loadAllCoreData: false)
        { }

        [Fact(DisplayName = "Do not place a competitor spot in pass 7001 for a timeband restriction")]
        public void DoNotPlaceTimebandCompetitorInPass7001()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                Path.Combine(
                    "SponsorshipExclusivity",
                    "UnplacedSpotPass"
                ));

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Spot("Spot_1").ShouldBePlaced().InBreak("Break_1");
                _ = testResults.Recommendations.Spot("Spot_2").ShouldBePlaced().InBreak("Break_1");
                _ = testResults.Recommendations.Spot("Spot_3").ShouldBePlaced().InBreak("Break_1");
                testResults.Recommendations.Spot("Spot_4").ShouldBeUnplaced();
                testResults.Recommendations.Spot("Spot_5").ShouldBeUnplaced();
                testResults.Recommendations.Spot("Spot_6").ShouldBeUnplaced();
                testResults.Recommendations.Spot("Spot_7").ShouldBeUnplaced();
                testResults.Recommendations.Spot("Spot_8").ShouldBeUnplaced();
                testResults.Recommendations.Spot("Spot_9").ShouldBeUnplaced();
                testResults.Recommendations.Spot("Spot_10").ShouldBeUnplaced();
            }
        }
    }
}
