using System;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Clash exposure counts")]
    public class ClashExposureCountTests : DataDrivenTests
    {
        public ClashExposureCountTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
        }

        protected override string SalesArea => "CEC01";

        protected override DateTimeRange SmoothPeriod => (
            start: new DateTime(2020, 3, 9, 0, 0, 0),
            end: new DateTime(2020, 3, 9, 23, 59, 59)
            );

        [Fact(DisplayName = "Programme in peak time places all spots in pass one")]
        public void ProgrammeInPeakTimePlacesAllSpotsInPassOne()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "ClashExposureCount"
                );

            var clashId = new Guid("D8DFBBD0-D413-4300-AADF-50F7C3719CB1");
            SetupClashForTest(clashId, 3);

            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                foreach (var value in testResults.Recommendations)
                {
                    _ = value.PassIterationSequence.Should().BeOneOf(1010, 1013);
                }
            }
        }

        [Fact(DisplayName = "Programme in peak time places one spot per break in pass one and the rest in pass five")]
        public void ProgrammeInPeakTimePlacesOneSpotPerBreakInPassOne()
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "ClashExposureCount"
                );

            var clashId = new Guid("D8DFBBD0-D413-4300-AADF-50F7C3719CB1");
            SetupClashForTest(clashId, 1);

            // Act
            ActThen(CheckResults);

            // Assert
            static void CheckResults(SmoothTestResults testResults)
            {
                const int PassSequenceOne = 1010;
                const int PassSequenceFive = 5001;

                _ = testResults.Recommendations.Should().HaveCount(10, becauseArgs: null);

                _ = testResults.Recommendations
                    .GroupBy(
                        r => r.ExternalBreakNo,
                        (key, group) => new
                        {
                            ExternalBreakReference = key,
                            PlacedSpotsCount = group.Count(r =>
                                r.PassIterationSequence == PassSequenceOne
                                )
                        }
                    )
                    .Should()
                    .OnlyContain(r => r.PlacedSpotsCount == 1, becauseArgs: null)
                    .And
                    .HaveCount(5, becauseArgs: null);

                _ = testResults.Recommendations
                    .GroupBy(
                        r => r.ExternalBreakNo,
                        (key, group) => new
                        {
                            ExternalBreakReference = key,
                            PlacedSpotsCount = group.Count(r =>
                                r.PassIterationSequence == PassSequenceFive
                                )
                        }
                    )
                    .Sum(r => r.PlacedSpotsCount)
                    .Should()
                    .Be(5, becauseArgs: null);
            }
        }

        private void SetupClashForTest(
            Guid clashId,
            int firstDifferencePeakExposureCount
            )
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IClashRepository>();

            Clash clash = repository.Get(clashId);
            clash.Differences[0].PeakExposureCount = firstDifferencePeakExposureCount;

            repository.Delete(clashId);
            repository.Add(clash);
            repository.SaveChanges();
        }
    }
}
