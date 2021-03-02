using System;
using System.Reflection;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Utilities.Tests.Data;
using NUnit.Framework;
using xggameplan.common.Utilities;
using static ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Utilities.Tests.Data.DaypartDayFormattingUtilitiesTestData;

namespace ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Utilities.Tests
{
    [TestFixture(Category = "Utilities :: Format DaypartDay")]
    public static class DaypartDayFormattingUtilitiesTests
    {
        [TestCaseSource(typeof(DaypartDayFormattingUtilitiesTestData), nameof(WeekDaysWithCustomStartTestCases))]
        [Test(Description = "Given a first week day then ordered array of week days returned")]
        public static void GivenAFirstWeekDayThenOrderedArrayOfWeekdaysReturned(DayOfWeek startDayOfWeek, string[] expectedWeek)
        {
            // Act
            var result = DaypartDayFormattingUtilities.GetWeekDaysWithCustomStart(startDayOfWeek);

            // Assert
            _ = result.Should().Equal(expectedWeek);
        }

        [Test(Description = "Given an invalid first input then an exception is thrown")]
        public static void GivenAnInvalidFirstInputThenAnExceptionIsThrown()
        {
            // Arrange
            string[] weekDays = null;
            var daypartDays = "YYYYYYY";

            Type type = typeof(DaypartDayFormattingUtilities);
            MethodBase myMethodBase = type.GetMethod(nameof(DaypartDayFormattingUtilities.FormatWeekDays));
            ParameterInfo[] parameterArray = myMethodBase.GetParameters();

            // Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                var result = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, daypartDays, DayOfWeek.Monday);
            });

            _ = ex.ParamName.Should().Be(parameterArray[0].Name, becauseArgs: null);
        }

        [Test(Description = "Given an invalid second input then an exception is thrown")]
        public static void GivenAnInvalidSecondInputThenAnExceptionIsThrown()
        {
            // Arrange
            var weekDays = DaypartDayFormattingUtilitiesTestData.TestWeek;
            string daypartDays = null;

            Type type = typeof(DaypartDayFormattingUtilities);
            MethodBase myMethodBase = type.GetMethod(nameof(DaypartDayFormattingUtilities.FormatWeekDays));
            ParameterInfo[] parameterArray = myMethodBase.GetParameters();

            // Assert
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                //Act
                var result = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, daypartDays, DayOfWeek.Monday);
            });

            _ = ex.ParamName.Should().Be(parameterArray[1].Name, becauseArgs: null);
        }

        [Test(Description = "Given input values are not compatible then an exception is thrown")]
        public static void GivenInputValuesAreNotCompatibleThenAnExceptionIsThrown()
        {
            // Arrange
            var weekDays = DaypartDayFormattingUtilitiesTestData.TestWeek;
            string daypartDays = "YYYY";

            // Assert
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                //Act
                var result = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, daypartDays, DayOfWeek.Monday);
            });

            _ = ex.Message.Should().Contain("Length", becauseArgs: null);
        }

        [TestCaseSource(typeof(DaypartDayFormattingUtilitiesTestData), nameof(FormatWeekDaysTestCases))]
        [Test(Description = "Given valid input values then formatted value of daypart day returned")]
        public static void GivenValidInputValuesThenFormattedValueOfDaypartDayReturned(
            string[] weekDays, string daypartDays, DayOfWeek startDayOfWeek, string expectedOutput)
        {
            // Act
            var result = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, daypartDays, startDayOfWeek);

            // Assert
            _ = result.Should().Be(expectedOutput, becauseArgs: null);
        }
    }
}
