using System;
using System.Reflection;
using FluentAssertions;
using xggameplan.common.Utilities;
using Xunit;
using static ImagineCommunications.GamePlan.Domain.XUnit.Tests.Utilities.Tests.DaypartDayFormattingUtilitiesTestData;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.Utilities.Tests
{
    [Trait("Utilities", "Format DaypartDay")]
    public static class DaypartDayFormattingUtilitiesTests
    {
        [Theory(DisplayName = "Given a first week day then ordered array of week days returned")]
        [MemberData(
            nameof(WeekDaysWithCustomStartTestCases),
            MemberType = typeof(DaypartDayFormattingUtilitiesTestData)
            )]
        public static void GivenAFirstWeekDayThenOrderedArrayOfWeekdaysReturned(
            DayOfWeek startDayOfWeek,
            string[] expectedWeek)
        {
            // Act
            var result = DaypartDayFormattingUtilities.GetWeekDaysWithCustomStart(startDayOfWeek);

            // Assert
            _ = result.Should().Equal(expectedWeek);
        }

        [Fact(DisplayName = "Given an invalid first input then an exception is thrown")]
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

        [Fact(DisplayName = "Given an invalid second input then an exception is thrown")]
        public static void GivenAnInvalidSecondInputThenAnExceptionIsThrown()
        {
            // Arrange
            var weekDays = TestWeek;
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

        [Fact(DisplayName = "Given input values are not compatible then an exception is thrown")]
        public static void GivenInputValuesAreNotCompatibleThenAnExceptionIsThrown()
        {
            // Arrange
            var weekDays = TestWeek;
            string daypartDays = "YYYY";

            // Assert
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                //Act
                var result = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, daypartDays, DayOfWeek.Monday);
            });

            _ = ex.Message.Should().Contain("Length", becauseArgs: null);
        }

        [Theory(DisplayName = "Given valid input values then formatted value of daypart day returned")]
        [MemberData(
            nameof(FormatWeekDaysTestCases),
            MemberType = typeof(DaypartDayFormattingUtilitiesTestData)
            )]
        public static void GivenValidInputValuesThenFormattedValueOfDaypartDayReturned(
            string[] weekDays,
            string daypartDays,
            DayOfWeek startDayOfWeek,
            string expectedOutput)
        {
            // Act
            var result = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, daypartDays, startDayOfWeek);

            // Assert
            _ = result.Should().Be(expectedOutput, becauseArgs: null);
        }
    }
}
