using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.DomainLogic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]

    [Trait("Smooth", "Add spots to break validator service")]
    public class AddSpotsToBreakValidatorServiceTests : DataDrivenTests
    {
        private readonly RepositoryWrapper _repositoryWrapper;

        private readonly Spot _spot = new Spot()
        {
            SpotLength = Duration.FromSeconds(15),
            BreakType = "BASE",
            StartDateTime = DateTime.Parse("2018-07-30T21:00:00"),
            EndDateTime = DateTime.Parse("2018-07-30T21:59:59")
        };

        public AddSpotsToBreakValidatorServiceTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, loadAllCoreData: true)
        {
            _repositoryWrapper = new RepositoryWrapper(RepositoryFactory);
        }

        [Theory(DisplayName =
            "Given a perfect break and spot " +
            "When checking if a spot can be added to the break " +
            "Then validator should not return any failures")]
        [InlineData(null, false, "2018-07-30T21:59:59", "1")]       // spot break type is NULL
        [InlineData("", false, "2018-07-30T21:59:59", "1")]         // spot break type is EMPTY string
        [InlineData("BASE", false, "2018-07-30T21:59:59", "1")]     // spot break type is the SAME as the break
        [InlineData(null, false, "2018-07-30T22:00:00", "1")]       // respect spot time is FALSE and spot end time is AFTER spot start time
        [InlineData(null, true, "2018-07-30T22:00:00", "1")]        // respect spot time is TRUE and spot end time is AFTER spot start time
        [InlineData(null, false, "2018-07-30T21:00:00", "1")]       // respect spot time is FALSE and spot end time is BEFORE spot start time
        [InlineData(null, true, "2018-07-30T20:00:00", "1")]        // respect spot time is TRUE and spot end time is BEFORE spot start time
        [InlineData(null, false, "2018-07-30T20:00:00", "1")]       // spot break request is the first break position
        [InlineData(null, false, "2018-07-30T20:00:00", "Break_1")] // spot break request is the SAME as break's external break reference
        [InlineData(null, false, "2018-07-30T20:00:00", null)]      // spot break request is null
        [InlineData(null, false, "2018-07-30T20:00:00", "")]        // spot break request is empty string
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsNoFailures(
            string spotBreakType,
            bool respectSpotTime,
            DateTime spotEndDateTime,
            string spotBreakRequest
            )
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            spotsForBreak[0].BreakType = spotBreakType;
            spotsForBreak[0].EndDateTime = spotEndDateTime;

            var placedSpot = (Spot)_spot.Clone();
            placedSpot.ExternalCampaignNumber = nameof(_spot.ExternalCampaignNumber);
            _ = theSmoothBreak.AddSpot(
                placedSpot,
                1,
                1,
                1,
                true,
                false,
                String.Empty,
                String.Empty);

            spotsForBreak[0].BreakRequest = spotBreakRequest;

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: true,
                respectSpotTime: respectSpotTime,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            _ = res.Should().ContainSingle();
            _ = res.First().Value.Failures.Should().BeEmpty();
        }

        [Fact(DisplayName =
            "Given a break with no availability " +
            "When checking if a spot can be added to the break " +
            "Then validator should return insufficient remaining duration failure")]
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsInsufficentRemainingDurationFailure()
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            theSmoothBreak.RemainingAvailability = Duration.Zero;

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: false,
                respectSpotTime: false,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            AssertFailureMessage(res, SmoothFailureMessages.T1_InsufficentRemainingDuration);
        }

        [Fact(DisplayName =
            "Given a break and spot have different break types " +
            "When checking if a spot can be added to the break " +
            "Then validator should return invalid break type failure")]
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsInvalidBreakTypeFailure()
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            spotsForBreak[0].BreakType = "DIFFERENT";

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: false,
                respectSpotTime: false,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            AssertFailureMessage(res, SmoothFailureMessages.T1_InvalidBreakType);
        }

        [Theory(DisplayName =
            "Given a break and spot time do not overlap " +
            "When checking if a spot can be added to the break " +
            "And the spot time is respected " +
            "Then validator should return invalid break type failure")]
        [InlineData("2018-07-30T20:00:00", "2018-07-30T20:59:59")] // Spot start and end time are before the break
        [InlineData("2018-07-30T22:00:00", "2018-07-30T22:59:59")] // Spot start and end time are after the break
        [InlineData("2018-07-30T22:00:00", "2018-07-30T20:00:00")] // Spot end time is before start time and spot time is after the break
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsInvalidSpotTimeFailure(
            DateTime spotStartDateTime,
            DateTime spotEndDateTime)
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            spotsForBreak[0].StartDateTime = spotStartDateTime;
            spotsForBreak[0].EndDateTime = spotEndDateTime;

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: false,
                respectSpotTime: true,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            AssertFailureMessage(res, SmoothFailureMessages.T1_InvalidSpotTime);
        }

        [Fact(DisplayName =
            "Given a break and spot clash " +
            "When checking if a spot can be added to the break " +
            "And the campaign clash is respected " +
            "Then validator should return invalid campaign clash failure")]
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsCampaignClashFailure()
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            _ = theSmoothBreak.AddSpot(
                (Spot)_spot.Clone(),
                1,
                1,
                1,
                true,
                false,
                String.Empty,
                String.Empty);

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: true,
                respectSpotTime: false,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            AssertFailureMessage(res, SmoothFailureMessages.T1_CampaignClash);
        }

        [Theory(DisplayName =
            "Given a break external break reference and spot break request are different " +
            "When checking if a spot can be added to the break " +
            "Then validator should return invalid break position failure")]
        [InlineData("2")]
        [InlineData("Break_2")]
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsBreakPositionFailure(string spotBreakRequest)
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            spotsForBreak[0].BreakRequest = spotBreakRequest;

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: true,
                respectSpotTime: false,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            AssertFailureMessage(res, SmoothFailureMessages.T1_BreakPosition);
        }

        [Fact(DisplayName =
            "Given a spot request in position is invalid " +
            "When checking if a spot can be added to the break " +
            "Then validator should return invalid break position failure")]
        public void AddSpotToBreakValidatorService_ValidateAddSpot_ReturnsRequestedPositionInBreakFailure()
        {
            // Arrange
            SetupTestData(
                out IReadOnlyCollection<Break> breaksBeingSmoothed,
                out SmoothBreak theSmoothBreak,
                out IReadOnlyCollection<Programme> scheduleProgrammes,
                out List<Spot> spotsForBreak,
                out SalesArea salesArea,
                out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
                out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
                out ProductClashRules productClashRule,
                out SmoothResources smoothResources,
                out IClashExposureCountService clashExposureCountService,
                out SponsorshipRestrictionService sponsorshipRestrictionsService);

            spotsForBreak[0].RequestedPositioninBreak = "INVALID";

            // Act
            var res = AddSpotsToBreakValidatorService.ValidateAddSpots(
                theSmoothBreak,
                programme: scheduleProgrammes.First(),
                salesArea: salesArea,
                spotsForBreak: spotsForBreak,
                spotInfos: spotInfos,
                progSmoothBreaks: new List<SmoothBreak> { theSmoothBreak },
                productClashRule: productClashRule,
                respectCampaignClash: true,
                respectSpotTime: false,
                respectRestrictions: false,
                respectClashExceptions: false,
                breakPositionRules: SpotPositionRules.Exact,
                requestedPositionInBreakRules: SpotPositionRules.Exact,
                clashesByExternalRef: clashesByExternalRef,
                canSplitMultipartSpotsOverBreaks: false,
                smoothResources: smoothResources,
                breaksBeingSmoothed: breaksBeingSmoothed,
                scheduleProgrammes: scheduleProgrammes,
                clashExposureCountService: clashExposureCountService,
                sponsorshipRestrictionsService: sponsorshipRestrictionsService);

            // Assert
            AssertFailureMessage(res, SmoothFailureMessages.T1_RequestedPositionInBreak);
        }

        private void SetupTestData(
            out IReadOnlyCollection<Break> breaksBeingSmoothed,
            out SmoothBreak theSmoothBreak,
            out IReadOnlyCollection<Programme> scheduleProgrammes,
            out List<Spot> spotsForBreak,
            out SalesArea salesArea,
            out IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            out IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            out ProductClashRules productClashRule,
            out SmoothResources smoothResources,
            out IClashExposureCountService clashExposureCountService,
            out SponsorshipRestrictionService sponsorshipRestrictionsService)
        {
            breaksBeingSmoothed = _repositoryWrapper.LoadAllTestBreaks().ToList();
            theSmoothBreak = new SmoothBreak(breaksBeingSmoothed.First(), 1);
            scheduleProgrammes = _repositoryWrapper.LoadAllProgrammes().ToList();
            spotsForBreak = new List<Spot>() { _spot };
            salesArea = _repositoryWrapper.LoadAllSalesArea().First();
            clashesByExternalRef = _repositoryWrapper.LoadAllClashes().ToDictionary(c => c.Externalref);
            spotInfos = SpotInfo.Factory(
                spotsForBreak,
                _repositoryWrapper.LoadAllProducts().ToDictionary(p => p.Externalidentifier),
                clashesByExternalRef
                );
            productClashRule = ProductClashRules.LimitOnExposureCount;
            smoothResources = new SmoothResources();
            clashExposureCountService = ClashExposureCountService.Create();
            sponsorshipRestrictionsService = SponsorshipRestrictionService.Factory(
                spotInfos,
                new SponsorshipRestrictionFilterService(ImmutableList.Create<Sponsorship>()),
                new SmoothSponsorshipTimelineManager(new List<SmoothSponsorshipTimeline>()),
                scheduleProgrammes.First(),
                DebugLogger);
        }

        private static void AssertFailureMessage(
            SmoothFailureMessagesForSpotsCollection res,
            SmoothFailureMessages message)
        {
            _ = res.Should().ContainSingle();
            _ = res.First().Value.Failures.Should().ContainSingle();
            _ = res.First().Value.Failures[0].FailureMessage.Should().Be(message);
        }

        private void DebugLogger(string value, Exception error)
        {
            _output.WriteLine(value);
            _output.WriteLine(error.Message);
        }
    }
}
