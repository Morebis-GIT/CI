using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Break container requests")]
    public class BreakContainerRequestTests : DataDrivenTests
    {
        private readonly RepositoryWrapper _repositoryWrapper;
        private readonly Fixture _fixture;

        private const string ValidBreakType = "VA-VALIDBREAKTYPE";
        private const string ContainerReferenceValue = "12345-6-7-LANDMARK-ONLY-DATA";

        private static Guid Spot_1Id => new Guid("00000001-0000-0000-0000-000000000000");

        public BreakContainerRequestTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output)
        {
            _fixture = new SafeFixture();
            _repositoryWrapper = new RepositoryWrapper(RepositoryFactory);
        }

        [Theory(DisplayName = "The requested container number for the sponsored spot is respected")]
        [InlineData("1", "Break-1-1")]
        [InlineData("1", "Break-1-2")]
        [InlineData("2", "Break-2-1")]
        [InlineData("2", "Break-2-2")]
        public void ContainerBreakRequestNumberForSponsoredSpot_Respected(
            string spotContainerRequest,
            string expectedContainerReference)
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "BreakRequest"
                );

            var allBreaks = _repositoryWrapper.LoadAllTestBreaks();
            int counter = 0;
            foreach (var aBreak in allBreaks.OrderBy(b => b.ExternalBreakRef))
            {
                int containerNumber = (counter / 2) + 1;
                int breakNumber = (counter % 2) + 1;
                counter++;

                aBreak.ExternalBreakRef = $"Break-{containerNumber.ToString()}-{breakNumber.ToString()}";

                _repositoryWrapper.SaveTestSample(aBreak);
            }

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);

            sampleSpot.BreakRequest = spotContainerRequest;
            sampleSpot.Sponsored = true;

            _repositoryWrapper.SaveTestSample(sampleSpot);

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InContainer(expectedContainerReference);
            }
        }

        [Theory(DisplayName = "The requested container number for the regular spot is respected")]
        [InlineData("1", "Break-1-1")]
        [InlineData("1", "Break-1-2")]
        [InlineData("2", "Break-2-1")]
        [InlineData("2", "Break-2-2")]
        public void ContainerBreakRequestNumberForRegularSpot_Respected(
            string spotContainerRequest,
            string expectedContainerReference)
        {
            // Arrange
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                "BreakRequest"
                );

            var allBreaks = _repositoryWrapper.LoadAllTestBreaks();
            int counter = 0;
            foreach (var aBreak in allBreaks.OrderBy(b => b.ExternalBreakRef))
            {
                int containerNumber = (counter / 2) + 1;
                int breakNumber = (counter % 2) + 1;
                counter++;

                aBreak.ExternalBreakRef = $"Break-{containerNumber.ToString()}-{breakNumber.ToString()}";

                _repositoryWrapper.SaveTestSample(aBreak);
            }

            var sampleSpot = _repositoryWrapper.LoadTestSpot(Spot_1Id);
            sampleSpot.BreakRequest = spotContainerRequest;
            _repositoryWrapper.SaveTestSample(sampleSpot);

            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                _ = testResults.Recommendations.Should().ContainSingle(becauseArgs: null);
                _ = testResults.Recommendations.Spot("Spot_1")
                    .ShouldBePlaced()
                    .InContainer(expectedContainerReference);
            }
        }

        private Break BreakWithValidBreakTypeFactory() =>
            _fixture
            .Build<Break>()
            .With(p => p.BreakType, ValidBreakType)
            .With(p => p.ScheduledDate, new DateTime(2020, 09, 24, 13, 30, 0))
            .With(p => p.Duration, Duration.FromMinutes(2))
            .With(p => p.ExternalBreakRef, ContainerReferenceValue)
            .Create();

        private Spot SpotWithValidBreakTypeAndStartTimeFactory() =>
            _fixture
            .Build<Spot>()
            .With(p => p.BreakType, ValidBreakType)
            .With(p => p.StartDateTime, new DateTime(2020, 09, 24, 13, 30, 15))
            .With(p => p.EndDateTime, new DateTime(2020, 09, 24, 13, 30, 45))
            .With(p => p.BreakRequest, ContainerReferenceValue)
            .Create();

        [Theory(DisplayName =
            "Given a spot with break container request that is different to the break container reference " +
            "When checking the spot can be placed " +
            "Then the spot cannot be placed")]
        [InlineData("DIFF", ContainerReferenceValue)]
        [InlineData("EX-2-3-MARK", ContainerReferenceValue)]
        [InlineData(ContainerReferenceValue, "DIFF")]
        [InlineData(ContainerReferenceValue, "EX-2-3-MARK")]
        [InlineData("2", "DIFF")]
        [InlineData("1", "EX-2-3-MARK")]
        public void CheckSpotCanBePlacedAtBreakContainerRequest_DifferentBreakContainerRequests_ReturnsFalse(
            string spotBreakContainerRequest,
            string breakExternalReference)
        {
            // Arrange
            var fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.ExternalBreakRef = breakExternalReference;

            var fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakRequest = spotBreakContainerRequest;

            SmoothProgramme smoothProgramme = _fixture
                .Build<SmoothProgramme>()
                .Create();

            smoothProgramme.InitialiseSmoothBreaks(
                new List<Break> { fakeBreak }
                );

            var smoothBreakList = smoothProgramme.ProgrammeSmoothBreaks;
            var fakeSmoothBreak = smoothBreakList[0];

            // Act
            var res = SpotUtilities.CanSpotBePlacedInRequestedBreakOrContainer(
                fakeSpot,
                smoothBreakList,
                SpotPositionRules.Exact,
                false,
                null,
                false,
                fakeSmoothBreak);

            // Assert
            _ = res.Should().BeFalse();
        }

        [Fact(DisplayName =
            "Given a spot with break container request that is requesting container number 1 " +
            "When checking the spot can be placed " +
            "Then the spot can be placed")]
        public void CheckSpotCanBePlacedAtBreakContainerRequest_RequestContainerNumberOne_ReturnsTrue()
        {
            // Arrange
            var fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.ExternalBreakRef = "12345-1-2";

            var fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakRequest = "1";

            SmoothProgramme smoothProgramme = _fixture
                .Build<SmoothProgramme>()
                .Create();

            smoothProgramme.InitialiseSmoothBreaks(
                new List<Break> { fakeBreak }
                );

            var smoothBreakList = smoothProgramme.ProgrammeSmoothBreaks;
            var fakeSmoothBreak = smoothBreakList[0];

            // Act
            var result = SpotUtilities.CanSpotBePlacedInRequestedBreakOrContainer(
                fakeSpot,
                smoothBreakList,
                SpotPositionRules.Exact,
                false,
                null,
                false,
                fakeSmoothBreak);

            // Assert
            _ = result.Should().BeTrue();
        }

        [Theory(DisplayName =
            "Given a spot with break container request that is the same as the break container reference " +
            "When checking the spot can be placed " +
            "Then the spot can be placed")]
        [InlineData("SAME", "SAME")]
        [InlineData(ContainerReferenceValue, ContainerReferenceValue)]
        [InlineData("6", ContainerReferenceValue)]
        public void CheckSpotCanBePlacedAtBreakContainerRequest_SameBreakRequests_ReturnsTrue(
            string spotBreakRequest,
            string breakExternalReference)
        {
            // Arrange
            var fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.ExternalBreakRef = breakExternalReference;

            var fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakRequest = spotBreakRequest;

            SmoothProgramme smoothProgramme = _fixture
                .Build<SmoothProgramme>()
                .Create();

            smoothProgramme.InitialiseSmoothBreaks(
                new List<Break> { fakeBreak }
                );

            var smoothBreakList = smoothProgramme.ProgrammeSmoothBreaks;
            var fakeSmoothBreak = smoothBreakList[0];

            // Act
            var result = SpotUtilities.CanSpotBePlacedInRequestedBreakOrContainer(
                fakeSpot,
                smoothBreakList,
                SpotPositionRules.Exact,
                false,
                null,
                false,
                fakeSmoothBreak);

            // Assert
            _ = result.Should().BeTrue();
        }

        [Theory(DisplayName =
            "Given a spot with no break container request " +
            "When checking the spot can be placed " +
            "Then the spot can be placed")]
        [InlineData("", "SAME")]
        [InlineData("      ", "SAME")]
        [InlineData(null, "SAME")]
        [InlineData("", ContainerReferenceValue)]
        [InlineData("      ", ContainerReferenceValue)]
        [InlineData(null, ContainerReferenceValue)]
        public void CheckSpotCanBePlacedAtBreakContainerRequest_NoBreakRequests_ReturnsTrue(
            string spotBreakRequest,
            string breakExternalReference)
        {
            // Arrange
            var fakeBreak = BreakWithValidBreakTypeFactory();
            fakeBreak.ExternalBreakRef = breakExternalReference;

            var fakeSpot = SpotWithValidBreakTypeAndStartTimeFactory();
            fakeSpot.BreakRequest = spotBreakRequest;

            SmoothProgramme smoothProgramme = _fixture
                .Build<SmoothProgramme>()
                .Create();

            smoothProgramme.InitialiseSmoothBreaks(
                new List<Break> { fakeBreak }
                );

            var smoothBreakList = smoothProgramme.ProgrammeSmoothBreaks;
            var fakeSmoothBreak = smoothBreakList[0];

            // Act
            var res = SpotUtilities.CanSpotBePlacedInRequestedBreakOrContainer(
                fakeSpot,
                smoothBreakList,
                SpotPositionRules.Exact,
                false,
                null,
                false,
                fakeSmoothBreak);

            // Assert
            _ = res.Should().BeTrue();
        }
    }
}
