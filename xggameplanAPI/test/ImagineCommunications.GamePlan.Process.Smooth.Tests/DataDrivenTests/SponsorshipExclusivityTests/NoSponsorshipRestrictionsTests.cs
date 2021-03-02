using System.IO;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "XG10/11 Sponsorship - No Sponsorship")]
    public class NoSponsorshipRestrictionsTests : SponsorshipRestrictionsTests
    {
        public NoSponsorshipRestrictionsTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, loadAllCoreData: false)
        { }

        [Fact(DisplayName = "Given no sponsorship records, Smooth will place all spots with all products")]
        internal void Given_no_sponsorship_records_Smooth_will_place_both_spots()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "SharedData"
                   )
               );

            DeleteAllSponsorshipRestrictionsForThisTest();

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                AssertRecommendationsAreSetWithNoSponsorshipRules(testResults.Recommendations);
            }
        }
    }
}
