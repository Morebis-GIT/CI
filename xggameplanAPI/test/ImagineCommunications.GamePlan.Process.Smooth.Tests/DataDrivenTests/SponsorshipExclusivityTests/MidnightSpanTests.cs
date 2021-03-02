using System;
using System.IO;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "XG10/11 Sponsorship - Midnight span")]
    public class MidnightSpanTests : SponsorshipRestrictionsTests
    {
        public MidnightSpanTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, loadAllCoreData: false)
        { }

        protected override DateTimeRange SmoothPeriod => new DateTimeRange(
            new DateTime(2020, 06, 09),
            new DateTime(2020, 06, 11)
            );

        [Fact(DisplayName = "Timeband - Flat - All competitors - Spot count")]
        internal void TimebandFlatAllCompetitorsSpotCount()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Flat-All-Count_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");

                _ = testResults.Recommendations.Spot("62")
                    .ShouldBePlaced()
                    .InBreak("2932777-2")
                    .AtActualPositionInBreak("1");

#pragma warning disable HAA0101 // Array allocation for params parameter
                testResults.Recommendations.Spots("63", "64", "65", "66", "67", "68", "69", "70")
                    .ShouldBeUnplaced();
#pragma warning restore HAA0101 // Array allocation for params parameter
            }
        }

        [Fact(DisplayName = "Timeband - Flat - All competitors - Spot duration")]
        internal void TimebandFlatAllCompetitorsSpotDuration()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Flat-All-Duration_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }

        [Fact(DisplayName = "Timeband - Flat - Each competitor - Spot count")]
        internal void TimebandFlatEachCompetitorSpotCount()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Flat-Each-Count_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }

        [Fact(DisplayName = "Timeband - Flat - Each competitor - Spot duration")]
        internal void TimebandFlatEachCompetitorSpotDuration()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Flat-Each-Duration_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }

        [Fact(DisplayName = "Timeband - Percentage - All competitors - Spot count")]
        internal void TimebandPercentageAllCompetitorsSpotCount()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Percentage-All-Count_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }

        [Fact(DisplayName = "Timeband - Percentage - All competitors - Spot duration")]
        internal void TimebandPercentageAllCompetitorsSpotDuration()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Percentage-All-Duration_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }

        [Fact(DisplayName = "Timeband - Percentage - Each competitor - Spot count")]
        internal void TimebandPercentageEachCompetitorSpotCount()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Percentage-Each-Count_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }

        [Fact(DisplayName = "Timeband - Percentage - Each competitor - Spot duration")]
        internal void TimebandPercentageEachCompetitorSpotDuration()
        {
            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
                Path.Combine(
                   "SponsorshipExclusivity",
                   "Timeband-Percentage-Each-Duration_MidnightSpan"
                   )
               );

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations.Spot("61")
                    .ShouldBePlaced()
                    .InBreak("2932777-1")
                    .AtActualPositionInBreak("1");
            }
        }
    }
}
