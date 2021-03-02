using System;
using System.Collections.Generic;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using NUnit.Framework;

namespace ImagineCommunications.GamePlan.Domain.Tests.DomainModels.Tests
{
    [TestFixture(Category = "Domain models :: " + nameof(Restriction))]
    public static class RestrictionTests
    {
        [TestCaseSource(nameof(_dayOfWeekTestDataSource))]
        [Description("Ensure the selected day of the week is returned")]
        public static void EnsureSelectedDayOfTheWeekIsReturned(
            in DayOfWeekTestData item)
        {
            // Arrange
            var sample = new Restriction
            {
                RestrictionDays = item.RestrictionDays
            };

            // Act
            IEnumerable<DayOfWeek> result = sample.RestrictionDaysOfWeek;

            // Assert
            _ = result.Should().ContainInOrder(item.ExpectedResult, "each restriction day is needed when checking spots can be placed in breaks");
        }

        private static readonly DayOfWeekTestData[] _dayOfWeekTestDataSource = {
            new DayOfWeekTestData(restrictionDays : null, expectedResult : new List<DayOfWeek>(0)),
            new DayOfWeekTestData(restrictionDays : String.Empty, expectedResult : new List<DayOfWeek>(0)),
            new DayOfWeekTestData(restrictionDays : "0000000", expectedResult : new List<DayOfWeek>(0)),
            new DayOfWeekTestData(restrictionDays : "1000000", expectedResult : new List<DayOfWeek> { DayOfWeek.Monday }),
            new DayOfWeekTestData(restrictionDays : "0100000", expectedResult : new List<DayOfWeek> { DayOfWeek.Tuesday }),
            new DayOfWeekTestData(restrictionDays : "0010000", expectedResult : new List<DayOfWeek> { DayOfWeek.Wednesday }),
            new DayOfWeekTestData(restrictionDays : "0001000", expectedResult : new List<DayOfWeek> { DayOfWeek.Thursday }),
            new DayOfWeekTestData(restrictionDays : "0000100", expectedResult : new List<DayOfWeek> { DayOfWeek.Friday }),
            new DayOfWeekTestData(restrictionDays : "0000010", expectedResult : new List<DayOfWeek> { DayOfWeek.Saturday }),
            new DayOfWeekTestData(restrictionDays : "0000001", expectedResult : new List<DayOfWeek> { DayOfWeek.Sunday }),
            new DayOfWeekTestData(restrictionDays : "1000001", expectedResult : new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Sunday }),
            new DayOfWeekTestData(restrictionDays : "1000100", expectedResult : new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday }),
            new DayOfWeekTestData(restrictionDays : "0101110", expectedResult : new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }),
            new DayOfWeekTestData(restrictionDays : "1111111", expectedResult : new List<DayOfWeek> { DayOfWeek.Monday,DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
        };

        public readonly struct DayOfWeekTestData
        {
            public DayOfWeekTestData(
                string restrictionDays,
                IEnumerable<DayOfWeek> expectedResult)
            {
                RestrictionDays = restrictionDays;
                ExpectedResult = expectedResult;
            }

            public readonly string RestrictionDays { get; }
            public readonly IEnumerable<DayOfWeek> ExpectedResult { get; }
        }
    }
}
