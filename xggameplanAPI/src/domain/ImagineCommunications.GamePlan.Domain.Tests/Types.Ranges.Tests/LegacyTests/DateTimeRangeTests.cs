#pragma warning disable CA1707 // Identifiers should not contain underscores
using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Tests.Types.Ranges.Data;
using NUnit.Framework;

namespace ImagineCommunications.GamePlan.Domain.Tests.Types.Ranges
{
    [TestFixture(Category = "Types :: DateTimeRange")]
    public static class DateTimeRangeTests
    {
        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.ContainsTestCases)
            )]
        [Description("Check if a DateTimeRange contains a specific DateTime")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.EqualsTestCases)
            )]
        [Description("Check if a DateTimeRange equals another DateTimeRange")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.EqualsTestCases)
            )]
        [Description("Check if a DateTimeRange equals operator matches another DateTimeRange")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.EqualsTestCases)
            )]
        [Description("Check if a DateTimeRange not equals operator matches another DateTimeRange")]
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
            Assert.That(result, Is.Not.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.HashCodeTestCases)
            )]
        [Description("Check if two DateTimeRange objects have equal hash codes")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.OverlayTestCases)
            )]
        [Description("Check if two DateTimeRange have overlay")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.ExplicitDateTimeConversionTestCases)
            )]
        [Description("Explicitly convert two DateTime objects to one DateTimeRange object")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.ExplicitDateTimeConversionTestCases)
            )]
        [Description("Explicitly convert one tuple object to one DateTimeRange object")]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(
            typeof(DateTimeRangeTestData),
            nameof(DateTimeRangeTestData.IteratorTestCases)
            )]
        [Description("Iterating over a date range returns all the dates")]
        public static void Iterating_over_a_date_range_must_return_all_the_dates(
            in DateTimeRange rangeToIterate,
            int expectedNumberOfDates
            )
        {
            // Arrange
            /* Empty */

            // Act
            int dayCount = rangeToIterate.Count();

            // Assert
            Assert.That(dayCount, Is.EqualTo(expectedNumberOfDates));
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
