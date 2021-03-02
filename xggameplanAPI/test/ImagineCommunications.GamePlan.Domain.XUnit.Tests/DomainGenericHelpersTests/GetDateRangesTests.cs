using System;
using System.Collections.Generic;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.DomainGenericHelpersTests
{
    [Trait("Domain Generic", "Helpers :: " + nameof(DateHelper))]
    public static class GetDateRangesTests
    {
        [Fact(DisplayName =
            "The minimum number of days to ask for in a date range is one day"
        )]
        public static void TooFewDaysInARangeThrows()
        {
            // Arrange
            // Empty

            // Act
            Action act = () =>
                _ = DateHelper.SplitUTCDateRange(default, 0);

            // Assert
            _ = act.Should().Throw<ArgumentOutOfRangeException>(
                    "a minimum of one day should be requested"
                    );
        }

        [Fact(DisplayName =
            "Given a single day date range; " +
            "When splitting the range into seven day chunks; " +
            "Then a single chunk is returned; " +
            "And the date range matches the single day."
            )]
        public static void SingleDayRange()
        {
            // Arrange
            DateTimeRange singleDay = (
                new DateTime(2030, 10, 31, 6, 0, 0),
                new DateTime(2030, 10, 31, 16, 0, 0)
                );

            // Act
            List<DateTimeRange> result = DateHelper.SplitUTCDateRange(singleDay, 7);

            // Assert
            _ = result.Should().ContainSingle(null);

            _ = result[0].Start.Date.Should().Be(new DateTime(2030, 10, 31).Date, null);
            _ = result[0].Start.TimeOfDay.Should().Be(new TimeSpan(6, 0, 0), null);
            _ = result[0].End.Date.Should().Be(new DateTime(2030, 10, 31).Date, null);
            _ = result[0].End.TimeOfDay.Should().Be(new TimeSpan(16, 0, 0), null);
        }

        [Fact(DisplayName =
            "Given a seven day date range; " +
            "When splitting the range into seven day chunks; " +
            "Then a single chunk is returned; " +
            "And the date range matches the seven days."
            )]
        public static void SevenDayRange()
        {
            // Arrange
            DateTimeRange sevenDays = (
                new DateTime(2030, 10, 24, 6, 0, 0),
                new DateTime(2030, 10, 24, 5, 59, 59).AddDays(6)
                );

            // Act
            List<DateTimeRange> result = DateHelper.SplitUTCDateRange(sevenDays, 7);

            // Assert
            _ = result.Should().HaveCount(1, null);

            _ = result[0].Start.Date.Should().Be(new DateTime(2030, 10, 24).Date, null);
            _ = result[0].Start.TimeOfDay.Should().Be(new TimeSpan(6, 0, 0), null);
            _ = result[0].End.Date.Should().Be(new DateTime(2030, 10, 30).Date, null);
            _ = result[0].End.TimeOfDay.Should().Be(new TimeSpan(5, 59, 59), null);
        }

        [Fact(DisplayName =
            "Given an eight day date range; " +
            "When splitting the range into seven day chunks; " +
            "Then two chunks are returned; " +
            "And the first date range matches the first seven days; " +
            "And the second date range matches the final day."
            )]
        public static void EightDayRange()
        {
            // Arrange
            DateTimeRange eightDays = (
                new DateTime(2030, 10, 24, 6, 0, 0),
                new DateTime(2030, 10, 24, 5, 59, 59).AddDays(7)
                );

            // Act
            List<DateTimeRange> result = DateHelper.SplitUTCDateRange(eightDays, 7);

            // Assert
            _ = result.Should().HaveCount(2, null);

            _ = result[0].Start.Date.Should().Be(new DateTime(2030, 10, 24).Date, null);
            _ = result[0].Start.TimeOfDay.Should().Be(new TimeSpan(6, 0, 0), null);
            _ = result[0].End.Date.Should().Be(new DateTime(2030, 10, 30).Date, null);
            _ = result[0].End.TimeOfDay.Should().Be(new TimeSpan(5, 59, 59), null);

            _ = result[1].Start.Date.Should().Be(new DateTime(2030, 10, 30).Date, null);
            _ = result[1].Start.TimeOfDay.Should().Be(new TimeSpan(6, 0, 0), null);
            _ = result[1].End.Date.Should().Be(new DateTime(2030, 10, 31).Date, null);
            _ = result[1].End.TimeOfDay.Should().Be(new TimeSpan(5, 59, 59), null);
        }
    }
}
