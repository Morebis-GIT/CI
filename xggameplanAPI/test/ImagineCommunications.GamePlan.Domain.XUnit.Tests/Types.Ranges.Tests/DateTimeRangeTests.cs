using System;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.Types.Ranges.Tests
{
    [Trait("Types", nameof(DateTimeRange))]
    public static class DateTimeRangeTests
    {
        [Theory(DisplayName = "Check if a DateTimeRange contains a specific DateTime")]
        [MemberData(
            nameof(DateTimeRangeTestData.ContainsTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Check_if_a_DateTimeRange_contains_a_specific_DateTime(
            DateTime dateRangeStart, DateTime dateRangeEnd,
            DateTime sample,
            bool expected
            )
        {
            // Arrange
            DateTimeRange labrat = (dateRangeStart, dateRangeEnd);

            // Act
            bool result = labrat.Contains(sample);

            // Assert
            _ = result.Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Check if a DateTimeRange equals another DateTimeRange")]
        [MemberData(
            nameof(DateTimeRangeTestData.EqualsTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Check_if_a_DateTimeRange_equals_another_DateTimeRange(
            in DateTimeRange dateRange,
            in DateTimeRange sample,
            bool expected
            )
        {
            // Arrange
            /* Nothing */

            // Act
            bool result = dateRange.Equals(sample);

            // Assert
            _ = result.Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Check if a DateTimeRange equals operator matches another DateTimeRange")]
        [MemberData(
            nameof(DateTimeRangeTestData.EqualsTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Check_if_a_DateTimeRange_equals_operator_matches_another_DateTimeRange(
            in DateTimeRange dateRange,
            in DateTimeRange sample,
            bool expected
            )
        {
            // Arrange
            /* Nothing */

            // Act
            bool result = dateRange == sample;

            // Assert
            _ = result.Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Check if a DateTimeRange not equals operator matches another DateTimeRange")]
        [MemberData(
            nameof(DateTimeRangeTestData.EqualsTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Check_if_a_DateTimeRange_not_equals_operator_matches_another_DateTimeRange(
            in DateTimeRange dateRange,
            in DateTimeRange sample,
            bool expected
            )
        {
            // Arrange
            /* Nothing */

            // Act
            bool result = dateRange != sample;

            // Assert
            _ = result.Should().Be(!expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Check if two DateTimeRange objects have equal hash codes")]
        [MemberData(
            nameof(DateTimeRangeTestData.HashCodeTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Check_if_two_DateTimeRange_objects_have_equal_hash_codes(
            in DateTimeRange firstValue,
            in DateTimeRange secondValue,
            bool expected
            )
        {
            // Arrange
            /* Nothing */

            // Act
            int firstHashCode = firstValue.GetHashCode();
            int secondHashCode = secondValue.GetHashCode();

            bool result = firstHashCode == secondHashCode;

            // Assert
            _ = result.Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Check if two DateTimeRange have overlay")]
        [MemberData(
            nameof(DateTimeRangeTestData.OverlayTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Check_if_two_DateTimeRange_objects_have_overlay(
            in DateTimeRange firstValue,
            in DateTimeRange secondValue,
            DateTimeRange.CompareStrategy compareStrategy,
            bool expected
            )
        {
            // Arrange
            /* Nothing */

            // Act
            bool result = firstValue.Overlays(secondValue, compareStrategy);

            // Assert
            _ = result.Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Explicitly convert two DateTime objects to one DateTimeRange object")]
        [MemberData(
            nameof(DateTimeRangeTestData.ExplicitDateTimeConversionTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Explicitly_convert_two_DateTime_objects_to_one_DateTimeRange_object(
            DateTime firstValue,
            DateTime secondValue,
            in DateTimeRange expected
            )
        {
            // Arrange
            /* Nothing */

            // Act
            var result = DateTimeRange.ToDateTimeRange(firstValue, secondValue);

            // Assert
            // Must use the As<T>() cast to avoid comparing as a collection
            _ = result.As<object>().Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Explicitly convert one tuple object to one DateTimeRange object")]
        [MemberData(
            nameof(DateTimeRangeTestData.ExplicitDateTimeConversionTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void
            Explicitly_convert_one_tuple_object_to_one_DateTimeRange_object(
            DateTime firstValue,
            DateTime secondValue,
            in DateTimeRange expected
            )
        {
            // Arrange
            (DateTime firstValue, DateTime secondValue) sample = (firstValue, secondValue);

            // Act
            var result = DateTimeRange.ToDateTimeRange(sample);

            // Assert
            // Must use the As<T>() cast to avoid comparing as a collection
            _ = result.As<object>().Should().Be(expected, becauseArgs: null);
        }

        [Theory(DisplayName = "Iterating over a date range returns all the dates")]
        [MemberData(
            nameof(DateTimeRangeTestData.IteratorTestCases),
            MemberType = typeof(DateTimeRangeTestData)
            )]
        public static void Iterating_over_a_date_range_must_return_all_the_dates(
            in DateTimeRange rangeToIterate,
            int expected
            )
        {
            // Arrange
            /* Empty */

            // Act
            int result = rangeToIterate.Count();

            // Assert
            _ = result.Should().Be(expected, becauseArgs: null);
        }
    }
}
