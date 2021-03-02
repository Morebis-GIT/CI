using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
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
        private readonly SafeFixture _fixture;

        public CheckRestrictionsTests(ITestOutputHelper output)
            : base(output)
        {
            _fixture = new SafeFixture();
        }

        [Fact(DisplayName =
            "Given no restrictions are configured; " +
            "When checking the spot for restrictions; " +
            "Then no restrictions are found"
            )]
        public void GivenNoRestrictionsSetupThenEmptyResultsReturned()
        {
            // Arrange
            IImmutableList<Restriction> noRestrictions =
                Enumerable.Empty<Restriction>().ToImmutableList();

            var restrictionChecker = new RestrictionChecker(
                noRestrictions, null, null, null, null, null);

            // Act
            var result = restrictionChecker.CheckRestrictions(
                null, null, null, null, null, null);

            // Assert
            _ = result.Should().BeEmpty("no restrictions found");
            _ = restrictionChecker.ExitReason.Should()
                    .Be(DebugRestrictionCheckerExitReason.NoRestrictionsToCheck, null);
        }

        [Fact(DisplayName =
            "Given the restriction and break are for different sales areas; " +
            "When checking the spot for restrictions; " +
            "Then no restrictions are found"
            )]
        public void RestrictionIsNotForThisBreakSalesArea()
        {
            // Arrange
            IImmutableList<Restriction> singleRestriction = SingleRestriction(
                startDate: new DateTime(2020, 10, 8, 10, 0, 0),
                endDate: new DateTime(2020, 10, 8, 10, 30, 0),
                daysOfTheWeek: "1000000",
                salesArea: "TyneTees");

            var sampleBreak = _fixture
                .Build<Break>()
                .With(p => p.SalesArea, "Channel4")
                .With(p => p.ScheduledDate, new DateTime(2020, 10, 8, 10, 0, 0))
                .Create();

            var sampleSalesArea = _fixture
                .Create<SalesArea>();

            var restrictionChecker = new RestrictionChecker(
                singleRestriction, null, null, null, null, null);

            // Act
            var result = restrictionChecker.CheckRestrictions(
                null, sampleBreak, null, sampleSalesArea, null, null);

            // Assert
            _ = result.Should().BeEmpty("no restrictions found");
            _ = restrictionChecker.ExitReason.Should()
                    .Be(DebugRestrictionCheckerExitReason.RestrictionIsNotForBreakSalesArea, null);
        }

        [Fact(DisplayName =
            "Given the break is outside the restriction's time span; " +
            "When checking the spot for restrictions; " +
            "Then no restrictions are found"
            )]
        public void BreakIsOutsideTheRestrictionTime()
        {
            // Arrange
            IImmutableList<Restriction> singleRestriction = SingleRestriction(
                startDate: new DateTime(2020, 10, 8, 10, 0, 0),
                endDate: new DateTime(2020, 10, 8, 10, 30, 0),
                daysOfTheWeek: "1000000",
                salesArea: "TyneTees");

            var sampleBreak = _fixture
                .Build<Break>()
                .With(p => p.SalesArea, "TyneTees")
                .With(p => p.ScheduledDate, new DateTime(2020, 10, 8, 11, 0, 0))
                .Create();

            var sampleSalesArea = _fixture
                .Create<SalesArea>();

            var restrictionChecker = new RestrictionChecker(
                singleRestriction, null, null, null, null, null);

            // Act
            var result = restrictionChecker.CheckRestrictions(
                null, sampleBreak, null, sampleSalesArea, null, null);

            // Assert
            _ = result.Should().BeEmpty("no restrictions found");
            _ = restrictionChecker.ExitReason.Should()
                .Be(DebugRestrictionCheckerExitReason.BreakIsOutsideTheRestrictionStartEndDateTime, null);
        }

        [Fact(DisplayName =
            "Given the break is on a day not covered by the restriction; " +
            "When checking the spot for restrictions; " +
            "Then no restrictions are found"
            )]
        public void BreakIsNotForTheRestrictionDay()
        {
            // Arrange
            IImmutableList<Restriction> singleRestriction = SingleRestriction(
                startDate: new DateTime(2020, 10, 8, 10, 0, 0),
                endDate: new DateTime(2020, 10, 18, 10, 30, 0),
                daysOfTheWeek: "1000000",
                salesArea: "TyneTees");

            var sampleBreak = _fixture
                .Build<Break>()
                .With(p => p.SalesArea, "TyneTees")
                .With(p => p.ScheduledDate, new DateTime(2020, 10, 8, 10, 0, 0))
                .Create();

            var sampleSalesArea = _fixture
                .Create<SalesArea>();

            var restrictionChecker = new RestrictionChecker(
                singleRestriction, null, null, null, null, null);

            // Act
            var result = restrictionChecker.CheckRestrictions(
                null, sampleBreak, null, sampleSalesArea, null, null);

            // Assert
            _ = result.Should().BeEmpty("no restrictions found");
            _ = restrictionChecker.ExitReason.Should()
                .Be(DebugRestrictionCheckerExitReason.RestrictionDoesNotCoverTheBreakDay, null);
        }

        private IImmutableList<Restriction> SingleRestriction(
            DateTime startDate,
            DateTime endDate,
            string daysOfTheWeek,
            string salesArea)
        {
            return _fixture
                .Build<Restriction>()
                .With(p => p.SalesAreas, new List<string> { salesArea })
                .With(p => p.StartDate, startDate.Date)
                .With(p => p.StartTime, startDate.TimeOfDay)
                .With(p => p.EndDate, endDate.Date)
                .With(p => p.EndTime, endDate.TimeOfDay)
                .With(p => p.RestrictionDays, daysOfTheWeek)
                .CreateMany(1)
                .ToImmutableList();
        }
    }
}
