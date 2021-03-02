using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    /// <summary>
    /// Tests for the spot restriction checker.
    /// </summary>
    [Trait("Smooth", "Check Restrictions")]
    public class CheckRestrictionsTests
        : TestBase
    {
        private const string ClashOneExternalRefCode = "clash_one";
        private const string ProductOneExternalId = "product_one";
        private const string ProgrammeOneExternalProgRef = "programme_one";

        /// <summary>The subject under test.</summary>
        private readonly RestrictionChecker _sut;

        /// <summary>Asserts the exit reason was there were no restrictions to check.</summary>
        private void AssertNoRestrictionsToCheck(RestrictionChecker sut)
        {
            AssertExitReason(
                sut,
                null,
                DebugExitReason_Restriction.NoRestrictionsToCheck
                );
        }

        /// <summary>Asserts the exit reason.</summary>
        /// <param name="restrictionToCheck">The unique id of a restriction to check.
        /// Pass <c>null</c> for no restrictions to check.</param>
        /// <param name="expectedExitReason">The expected exit reason.</param>
        private void AssertExitReason(
            RestrictionChecker sut,
            string restrictionToCheck,
            DebugExitReason_Restriction expectedExitReason
            )
        {
            Guid restrictionId = restrictionToCheck is null
                ? Guid.Empty
                : GetRestrictionId(restrictionToCheck);

            _ = sut.ExitReasons.First(x => x.restrictionId == restrictionId)
                    .exitReason.Should()
                    .Be(expectedExitReason, null);
        }

        private readonly IDictionary<string, Guid> _restrictionsIdCache
            = new Dictionary<string, Guid>();

        /// <summary>Gets the restriction instance by instance name.</summary>
        /// <param name="restrictionInstanceName">Name of the restriction instance.</param>
        /// <returns></returns>
        private Guid GetRestrictionId(string restrictionInstanceName) =>
            _restrictionsIdCache[restrictionInstanceName];

        private static readonly Restriction _basicRestriction = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.RestrictionType, RestrictionType.Time)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .Create();

        private static readonly Restriction _excludeLiveProgrammeRestriction = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.RestrictionType, RestrictionType.Programme)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .With(p => p.LiveProgrammeIndicator, IncludeOrExclude.E)
            .Create();

        private static readonly Restriction _schoolHolidayIncludedRestriction = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.I)
            .With(p => p.RestrictionType, RestrictionType.Time)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .Create();

        private static readonly Restriction _schoolHolidayExcludedRestriction = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.E)
            .With(p => p.RestrictionType, RestrictionType.Time)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .Create();

        private static readonly Restriction _publicHolidayIncludedRestriction = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.I)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.RestrictionType, RestrictionType.Time)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .Create();

        private static readonly Restriction _publicHolidayExcludedRestriction = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.E)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.RestrictionType, RestrictionType.Time)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .Create();

        private static readonly Restriction _restrictionTypeTimeMatchesClashOneCode = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.RestrictionType, RestrictionType.Time)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .With(p => p.ClashCode, ClashOneExternalRefCode)
            .With(p => p.TimeToleranceMinsBefore, 15)
            .With(p => p.TimeToleranceMinsAfter, 15)
            .Create();
        private static readonly Restriction _restrictionTypeProgrammeMatchesClashOneCode = Fixture
            .Build<Restriction>()
            .With(p => p.SalesAreas, new List<string> { "Channel4" })
            .With(p => p.StartDate, new DateTime(2020, 12, 1))
            .With(p => p.EndDate, new DateTime(2020, 12, 31))
            .With(p => p.StartTime, new TimeSpan(15, 00, 00))
            .With(p => p.EndTime, new TimeSpan(21, 00, 00))
            .With(p => p.RestrictionDays, "1000000")
            .With(p => p.PublicHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.SchoolHolidayIndicator, IncludeOrExcludeOrEither.X)
            .With(p => p.RestrictionType, RestrictionType.Programme)
            .With(p => p.RestrictionBasis, RestrictionBasis.Clash)
            .With(p => p.ClashCode, ClashOneExternalRefCode)
            .With(p => p.ExternalProgRef, ProgrammeOneExternalProgRef)
            .With(p => p.TimeToleranceMinsBefore, 15)
            .With(p => p.TimeToleranceMinsAfter, 15)
            .Create();

        public CheckRestrictionsTests(ITestOutputHelper output)
                : base(output)
        {
            var restrictions = new[] {
                _basicRestriction,
                _excludeLiveProgrammeRestriction,
                _schoolHolidayExcludedRestriction,
                _schoolHolidayIncludedRestriction,
                _publicHolidayIncludedRestriction,
                _publicHolidayExcludedRestriction,
                _restrictionTypeTimeMatchesClashOneCode,
                _restrictionTypeProgrammeMatchesClashOneCode
            }.ToImmutableList();

            _restrictionsIdCache.Add(nameof(_basicRestriction), _basicRestriction.Uid);
            _restrictionsIdCache.Add(nameof(_excludeLiveProgrammeRestriction), _excludeLiveProgrammeRestriction.Uid);
            _restrictionsIdCache.Add(nameof(_schoolHolidayExcludedRestriction), _schoolHolidayExcludedRestriction.Uid);
            _restrictionsIdCache.Add(nameof(_schoolHolidayIncludedRestriction), _schoolHolidayIncludedRestriction.Uid);
            _restrictionsIdCache.Add(nameof(_publicHolidayIncludedRestriction), _publicHolidayIncludedRestriction.Uid);
            _restrictionsIdCache.Add(nameof(_publicHolidayExcludedRestriction), _publicHolidayExcludedRestriction.Uid);
            _restrictionsIdCache.Add(nameof(_restrictionTypeTimeMatchesClashOneCode), _restrictionTypeTimeMatchesClashOneCode.Uid);
            _restrictionsIdCache.Add(nameof(_restrictionTypeProgrammeMatchesClashOneCode), _restrictionTypeProgrammeMatchesClashOneCode.Uid);

            var oneClash = Fixture
                .Build<Clash>()
                .With(p => p.Externalref, ClashOneExternalRefCode)
                .Create();

            var clashes = new[]
            {
                oneClash
            };

            var productOne = Fixture
                .Build<Product>()
                .With(p => p.Externalidentifier, ProductOneExternalId)
                .With(p => p.ClashCode, oneClash.Externalref)
                .Create();

            var products = new[] {
                productOne
            }.ToList();

            _sut = new RestrictionChecker(
                restrictions,
                products,
                clashes,
                null,
                null,
                null);
        }

        [Fact(DisplayName =
            "Given no restrictions are configured " +
            "When checking for restrictions " +
            "Then no restrictions are found"
            )]
        public void GivenNoRestrictionsSetupThenEmptyResultsReturned()
        {
            // Arrange
            var noRestrictions = ImmutableList<Restriction>.Empty;
            var salesArea = Fixture.Create<SalesArea>();

            var sut = new RestrictionChecker(
                noRestrictions,
                null,
                null,
                null,
                null,
                null);

            // Act
            var result = sut.CheckRestrictions(
                null,
                null,
                null,
                salesArea,
                null,
                null);

            // Assert
            AssertNoRestrictionsToCheck(sut);
            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given a null sales area object " +
            "When checking for spot restrictions " +
            "Then an exception is thrown."
            )]
        public void NullSalesAreaObjectThrowsException()
        {
            // Arrange
            /* Empty */

            // Act
            Action act = () =>
            {
                var result = _sut.CheckRestrictions(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);
            };

            // Assert
            _ = act.Should()
                    .Throw<ArgumentNullException>(null)
                    .WithMessage("*Parameter *salesArea*", null);
        }

        [Fact(DisplayName =
            "Given a null break object " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakIsNullSoDoesNotMatchAnyRestriction()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();

            // Act
            var result = _sut.CheckRestrictions(
                null,
                null,
                null,
                salesArea,
                null,
                null);

            // Assert
            AssertNoRestrictionsToCheck(_sut);
            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given a break not in the restriction's sales area " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakDoesNotMatchTheRestrictionSalesArea()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "DoNotFind")
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                null,
                theBreak,
                null,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_basicRestriction),
                DebugExitReason_Restriction.IsNotForBreakSalesArea);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given a break not in the restriction's date time window " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakDoesNotMatchTheRestrictionDateTimeWindow()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, new DateTime(2020, 12, 25, 13, 00, 00))
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                null,
                theBreak,
                null,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_basicRestriction),
                DebugExitReason_Restriction.StartEndDateTimeDoesNotContainTheBreak);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given a break not in the restriction's days of the week " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakDoesNotMatchTheRestrictionDaysOfTheWeekWindow()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")

                // Scheduled day is a Friday; Restriction is for Monday
                .With(p => p.ScheduledDate, new DateTime(2020, 12, 25, 18, 00, 00))
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                null,
                theBreak,
                null,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_basicRestriction),
                DebugExitReason_Restriction.DoesNotCoverTheBreakDay);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given the break is during a school holiday " +
            "But the restriction excludes school holidays " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakIsDuringSchoolHolidayButRestrictionExcludesSchoolHolidays()
        {
            // Arrange
            var schoolHoliday = new DateRange
            {
                Start = new DateTime(2020, 12, 24),
                End = new DateTime(2021, 1, 4)
            };

            var salesArea = Fixture.Create<SalesArea>();
            salesArea.SchoolHolidays = new List<DateRange> {
                schoolHoliday
            };

            var breakFallsOnMondayWithinSchoolHoliday = new DateTime(2020, 12, 28, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayWithinSchoolHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, "do_not_find_me")
                .Create();

            var programme = Fixture
                .Build<Programme>()
                .With(p => p.LiveBroadcast, false)
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_schoolHolidayExcludedRestriction),
                DebugExitReason_Restriction.AppliesToBreaksOutsideSchoolHolidays);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given the break is not during a school holiday " +
            "And the restriction includes school holidays " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakIsNotDuringSchoolHolidayAndRestrictionIncludesSchoolHolidays()
        {
            // Arrange
            var schoolHoliday = new DateRange
            {
                Start = new DateTime(2020, 12, 24),
                End = new DateTime(2021, 1, 4)
            };

            var salesArea = Fixture.Create<SalesArea>();
            salesArea.SchoolHolidays = new List<DateRange> {
                schoolHoliday
            };

            var breakFallsOnMondayOutsideSchoolHoliday = new DateTime(2020, 12, 21, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayOutsideSchoolHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, "do_not_find_me")
                .Create();

            var programme = Fixture
                .Build<Programme>()
                .With(p => p.LiveBroadcast, false)
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_schoolHolidayIncludedRestriction),
                DebugExitReason_Restriction.AppliesToBreaksWithinSchoolHolidays);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given the break is during a public holiday " +
            "But the restriction excludes public holidays " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakIsDuringPublicHolidayButRestrictionExcludesPublicHolidays()
        {
            // Arrange
            var publicHoliday = new DateRange
            {
                Start = new DateTime(2020, 12, 24),
                End = new DateTime(2021, 1, 4)
            };

            var salesArea = Fixture.Create<SalesArea>();
            salesArea.PublicHolidays = new List<DateRange> {
                publicHoliday
            };

            var breakFallsOnMondayWithinPublicHoliday = new DateTime(2020, 12, 28, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayWithinPublicHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, "do_not_find_me")
                .Create();

            var programme = Fixture
                .Build<Programme>()
                .With(p => p.LiveBroadcast, false)
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_publicHolidayExcludedRestriction),
                DebugExitReason_Restriction.AppliesToBreaksOutsidePublicHolidays);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given the break is not during a public holiday " +
            "And the restriction includes public holidays " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void BreakIsNotDuringPublicHolidayAndRestrictionIncludesPublicHolidays()
        {
            // Arrange
            var publicHoliday = new DateRange
            {
                Start = new DateTime(2020, 12, 24),
                End = new DateTime(2021, 1, 4)
            };

            var salesArea = Fixture.Create<SalesArea>();
            salesArea.PublicHolidays = new List<DateRange> {
                publicHoliday
            };

            var breakFallsOnMondayOutsidePublicHoliday = new DateTime(2020, 12, 21, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayOutsidePublicHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, "do_not_find_me")
                .Create();

            var programme = Fixture
                .Build<Programme>()
                .With(p => p.LiveBroadcast, false)
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_publicHolidayIncludedRestriction),
                DebugExitReason_Restriction.AppliesToBreaksWithinPublicHolidays);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given the programme is a live broadcast " +
            "But the restriction excludes checking live broadcasts " +
            "When checking for spot restrictions " +
            "Then no restrictions are found."
            )]
        public void ProgrammeIsLiveBroadcastButTheRestrictionExcludesCheckingLiveBroadcasts()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();

            var breakFallsOnMondayOutsidePublicHoliday = new DateTime(2020, 12, 21, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayOutsidePublicHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, "do_not_find_me")
                .Create();

            var programme = Fixture
                .Build<Programme>()
                .With(p => p.LiveBroadcast, true)
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                null,
                null);

            // Assert
            AssertExitReason(
                _sut,
                nameof(_excludeLiveProgrammeRestriction),
                DebugExitReason_Restriction.DoesNotApplyToLiveBroadcasts);

            _ = result.Should().BeEmpty("no restrictions found");
        }

        [Fact(DisplayName =
            "Given a restriction by clash code on a time basis " +
            "When checking for spot restrictions " +
            "Then a time restriction for clash is returned."
            )]
        public void TimeRestrictionForClash()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();

            var breakFallsOnMondayOutsidePublicHoliday = new DateTime(2020, 12, 21, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayOutsidePublicHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, ProductOneExternalId)
                .Create();

            var programmeStartDateTime = new DateTime(2020, 12, 21, 17, 30, 00);
            var programme = Fixture
                .Build<Programme>()
                .With(p => p.LiveBroadcast, false)
                .With(p => p.StartDateTime, programmeStartDateTime)
                .With(p => p.Duration, Duration.FromHours(1))
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                new List<Break> { theBreak },
                new List<Programme> { programme }
                );

            // Assert
            AssertExitReason(
                _sut,
                nameof(_restrictionTypeTimeMatchesClashOneCode),
                DebugExitReason_Restriction.EndOfMethod);

            var expected = new CheckRestrictionResult(
                _restrictionTypeTimeMatchesClashOneCode,
                RestrictionReasons.TimeRestrictionForClash
                );

            _ = result.Should().Contain(expected, null);
        }

        [Fact(DisplayName =
            "Given a restriction by clash code on a programme basis " +
            "When checking for spot restrictions " +
            "Then a time restriction for clash is returned."
            )]
        public void ProgrammeRestrictionForClash()
        {
            // Arrange
            var salesArea = Fixture.Create<SalesArea>();

            var breakFallsOnMondayOutsidePublicHoliday = new DateTime(2020, 12, 21, 18, 00, 00);
            var theBreak = Fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, breakFallsOnMondayOutsidePublicHoliday)
                .Create();

            var spot = Fixture
                .Build<Spot>()
                .With(p => p.Product, ProductOneExternalId)
                .Create();

            var programmeStartDateTime = new DateTime(2020, 12, 21, 17, 30, 00);
            var programme = Fixture
                .Build<Programme>()
                .With(p => p.ExternalReference, ProgrammeOneExternalProgRef)
                .With(p => p.LiveBroadcast, false)
                .With(p => p.StartDateTime, programmeStartDateTime)
                .With(p => p.Duration, Duration.FromHours(1))
                .Create();

            // Act
            var result = _sut.CheckRestrictions(
                programme,
                theBreak,
                spot,
                salesArea,
                new List<Break> { theBreak },
                new List<Programme> { programme }
                );

            // Assert
            AssertExitReason(
                _sut,
                nameof(_restrictionTypeProgrammeMatchesClashOneCode),
                DebugExitReason_Restriction.EndOfMethod);

            var expected = new CheckRestrictionResult(
                _restrictionTypeProgrammeMatchesClashOneCode,
                RestrictionReasons.ProgrammeRestrictionForClash
                );

            _ = result.Should().Contain(expected, null);
        }
    }
}
