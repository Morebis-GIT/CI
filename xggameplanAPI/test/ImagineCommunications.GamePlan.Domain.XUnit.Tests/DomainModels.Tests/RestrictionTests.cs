using System;
using System.Collections.Generic;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.DomainModels.Tests
{
    [Trait("Domain models", nameof(Restriction))]
    public static class RestrictionTests
    {
        [Theory(DisplayName = "Ensure the selected day of the week is returned")]
        [MemberData(
            nameof(_dayOfWeekTestDataSource),
            MemberType = typeof(RestrictionTests)
            )]
        public static void EnsureSelectedDayOfTheWeekIsReturned(
            string restrictionDays,
            IEnumerable<DayOfWeek> expectedResult
            )
        {
            // Arrange
            var sample = new Restriction
            {
                RestrictionDays = restrictionDays
            };

            // Act
            IEnumerable<DayOfWeek> result = sample.RestrictionDaysOfWeek;

            // Assert
            _ = result.Should().ContainInOrder(
                expectedResult,
                "each restriction day is needed when checking spots can be placed in breaks");
        }

        public static readonly IEnumerable<object[]> _dayOfWeekTestDataSource = new[]{
            new object[]{ null, new List<DayOfWeek>(0)},
            new object[]{ String.Empty, new List<DayOfWeek>(0) },
            new object[]{ "0000000", new List<DayOfWeek>(0) },
            new object[]{ "1000000", new List<DayOfWeek> { DayOfWeek.Monday }},
            new object[]{ "0100000", new List<DayOfWeek> { DayOfWeek.Tuesday }},
            new object[]{ "0010000", new List<DayOfWeek> { DayOfWeek.Wednesday }},
            new object[]{ "0001000", new List<DayOfWeek> { DayOfWeek.Thursday }},
            new object[]{ "0000100", new List<DayOfWeek> { DayOfWeek.Friday }},
            new object[]{ "0000010", new List<DayOfWeek> { DayOfWeek.Saturday }},
            new object[]{ "0000001", new List<DayOfWeek> { DayOfWeek.Sunday }},
            new object[]{ "1000001", new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Sunday }},
            new object[]{ "1000100", new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday }},
            new object[]{ "0101110", new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }},
            new object[]{ "1111111", new List<DayOfWeek> { DayOfWeek.Monday,DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }}
        };
    }
}
