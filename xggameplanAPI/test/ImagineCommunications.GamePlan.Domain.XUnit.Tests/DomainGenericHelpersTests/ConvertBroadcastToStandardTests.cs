using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.DomainGenericHelpersTests
{
    [Trait("Domain Generic", "Helpers :: DateHelper")]
    public static class ConvertBroadcastToStandardTests
    {
        [Fact(DisplayName =
            "Given a datetime object and timespan object " +
            "then the datetime object should be incremented by the value of the timespan if the timespan is >= 0600."
        )]
        public static void Given_datetime_and_timespan_then_datetime_should_increment_by_value_of_timespan_if_timespan_equal_or_later_than_0600()
        {
            // Arrange
            var timeSpan = new TimeSpan(06, 59, 59);
            var dateTime = new DateTime(2018, 07, 18);

            // Act
            DateTime result = DateHelper.ConvertBroadcastToStandard(dateTime, timeSpan);

            // Assert
            _ = result.Should().Equals(dateTime.Add(timeSpan));
        }

        [Fact(DisplayName =
            "Given a datetime object and timespan object " +
            "then the datetime object should be incremented by the value of the timespan object and 1 day should be added if the timespan is before 0600."
        )]
        public static void Given_datetime_and_timespan_then_datetime_should_increment_by_value_of_timespan_and_1_day_added_if_timespan_before_0600()
        {
            // Arrange
            var timeSpan = new TimeSpan(05, 59, 59);
            var dateTime = new DateTime(2018, 07, 18);

            // Act
            DateTime result = DateHelper.ConvertBroadcastToStandard(dateTime, timeSpan);

            // Assert
            _ = result.Should().Equals(dateTime.AddDays(1).Add(timeSpan));
        }
    }
}
