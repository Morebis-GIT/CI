using System.IO;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    /// <summary>
    /// <para>Test the use cases for</para>
    /// <list type="bullet">
    /// <item>Restriction Level: Programme</item>
    /// <item>Calculation type: Exclusive</item>
    /// </list>
    /// <listheader>Use cases</listheader>
    /// <list type="number">
    /// <item>Do not place any competitor spots in sponsored breaks.</item>
    /// </list>
    /// </summary>
    public sealed class ProgrammeExclusiveTests
    {
        [Collection(nameof(SmoothCollectionDefinition))]
        [Trait("Smooth", "XG10/11 Sponsorship - Programme - Exclusive")]
        public sealed class SingleBreaks : SponsorshipRestrictionsTests
        {
            public SingleBreaks(
                SmoothFixture smoothFixture,
                ITestOutputHelper output
                )
                : base(smoothFixture, output)
            {
            }

            [Theory(DisplayName = "If a single break contains a sponsored spot do not place any competitor spots in the break.")]
            [InlineData("Exclusive_UC1_1BreakByAdCode")]
            [InlineData("Exclusive_UC1_1BreakByClashCode")]
            public void If_a_single_break_contains_a_sponsored_spot_do_not_place_any_competitor_spots_in_the_break(
                string dataSet)
            {
                DataLoader.LoadTestSpecificRepositories(
                    RepositoryFactory,
                    Path.Combine("SponsorshipExclusivity", "Programme_Exclusive", dataSet)
                    );

                ActThen(CheckResults);

                // Assert
                void CheckResults(SmoothTestResults testResults)
                {
                    DumpRecommentationsToDebug(testResults.Recommendations);

                    _ = testResults.Recommendations.Should().HaveCount(2, becauseArgs: null);

                    _ = testResults.Recommendations.Spot("Spot_1")
                        .ShouldBePlaced()
                        .InBreak("Break_1")
                        .AtActualPositionInBreak("1");

                    testResults.Recommendations.Spot("Spot_2")
                        .ShouldBeUnplaced();
                }
            }

            [Theory(DisplayName = "If a break does not contain a sponsored spot still do not place any competitor spots.")]
            [InlineData("Exclusive_UC2_1BreakByAdCode")]
            [InlineData("Exclusive_UC2_1BreakByClashCode")]
            public void If_a_break_does_not_contain_a_sponsored_spot_still_do_not_place_any_competitor_spots(
                string dataSet)
            {
                DataLoader.LoadTestSpecificRepositories(
                    RepositoryFactory,
                    Path.Combine("SponsorshipExclusivity", "Programme_Exclusive", dataSet)
                    );

                ActThen(CheckResults);

                // Assert
                void CheckResults(SmoothTestResults testResults)
                {
                    DumpRecommentationsToDebug(testResults.Recommendations);

                    _ = testResults.Recommendations.Should().HaveCount(2, becauseArgs: null);

                    _ = testResults.Recommendations.Spot("Spot_1")
                        .ShouldBePlaced()
                        .InBreak("Break_1")
                        .AtActualPositionInBreak("1");

                    testResults.Recommendations.Spot("Spot_2")
                        .ShouldBeUnplaced();
                }
            }
        }

        [Collection(nameof(SmoothCollectionDefinition))]
        [Trait("Smooth", "XG10/11 Sponsorship - Programme - Exclusive")]
        public sealed class TwoBreaks : SponsorshipRestrictionsTests
        {
            public TwoBreaks(
                SmoothFixture smoothFixture,
                ITestOutputHelper output)
                : base(smoothFixture, output)
            {
            }

            [Theory(DisplayName = "If one of two breaks contains a sponsored spot do not place any competitor spots in the break.")]
            [InlineData("Exclusive_UC3_2BreaksByAdCode")]
            [InlineData("Exclusive_UC3_2BreaksByClashCode")]
            public void If_one_of_two_breaks_contains_a_sponsored_spot_do_not_place_any_competitor_spots_in_the_break(
                string dataSet)
            {
                DataLoader.LoadTestSpecificRepositories(
                    RepositoryFactory,
                    Path.Combine("SponsorshipExclusivity", "Programme_Exclusive", dataSet)
                    );

                ActThen(CheckResults);

                // Assert
                void CheckResults(SmoothTestResults testResults)
                {
                    DumpRecommentationsToDebug(testResults.Recommendations);

                    _ = testResults.Recommendations.Should().HaveCount(3, becauseArgs: null);

                    _ = testResults.Recommendations.Spot("Spot_1")
                        .ShouldBePlaced()
                        .InBreak("Break_1")
                        .AtActualPositionInBreak("1");

                    testResults.Recommendations.Spot("Spot_2")
                        .ShouldBeUnplaced();

                    testResults.Recommendations.Spot("Spot_3")
                        .ShouldBeUnplaced();
                }
            }
        }
    }
}
