using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.Types.Ranges.Tests
{
    public sealed class DateTimeRangeTestData
    {
        private readonly static DateTime _emptyDate = new DateTime();
        private readonly static DateTime _halloween = new DateTime(2018, 10, 31);
        private readonly static DateTime _diwali = new DateTime(2018, 11, 7);
        private readonly static DateTime _christmasDay = new DateTime(2018, 12, 25);
        private readonly static DateTime _newYearsDay = new DateTime(2019, 1, 1);

        public readonly static IEnumerable<object[]> ExplicitDateTimeConversionTestCases = new[]{
            new object[] { _emptyDate, _emptyDate, new DateTimeRange(_emptyDate, _emptyDate) },
            new object[] { _christmasDay, _emptyDate, new DateTimeRange(_christmasDay, _emptyDate) },
            new object[] { _emptyDate, _christmasDay, new DateTimeRange(_emptyDate, _christmasDay) },
            new object[] { _christmasDay, _newYearsDay, new DateTimeRange(_christmasDay, _newYearsDay) }
        };

        public readonly static IEnumerable<object[]> ContainsTestCases = new[]{
            new object[] { _emptyDate, _emptyDate, _emptyDate, true },
            new object[] { _christmasDay, _emptyDate, _emptyDate, false },
            new object[] { _emptyDate, _christmasDay, _emptyDate, true },
            new object[] { _emptyDate, _emptyDate, _christmasDay, false },

            new object[] { _christmasDay, _christmasDay, _diwali, false },
            new object[] { _christmasDay, _christmasDay, _christmasDay, true },
            new object[] { _christmasDay, _christmasDay, _newYearsDay, false },

            new object[] { _diwali, _christmasDay, _halloween, false },
            new object[] { _diwali, _christmasDay, _diwali, true },
            new object[] { _diwali, _christmasDay, _christmasDay, true },
            new object[] { _diwali, _christmasDay, _newYearsDay, false },

            new object[] { _christmasDay, _newYearsDay, _christmasDay, true },
            new object[] { _newYearsDay, _newYearsDay, _christmasDay, false },

            // End date is before start date
            new object[] { _newYearsDay, _christmasDay, _christmasDay, false },

            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 6), false },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 5), new DateTime(2018, 12, 25, 12, 59, 0), false },

            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 6), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 5), new DateTime(2018, 12, 25, 12, 59, 0), false },
        };

        public readonly static IEnumerable<object[]> EqualsTestCases = new[]{
            new object[] { new DateTimeRange(), new DateTimeRange(), true },
            new object[] { new DateTimeRange(_christmasDay, _newYearsDay), new DateTimeRange(), false },
            new object[] { new DateTimeRange(), new DateTimeRange(_christmasDay, _newYearsDay),  false },
            new object[] { new DateTimeRange(_diwali, _christmasDay), new DateTimeRange(_christmasDay, _newYearsDay), false },
            new object[] { new DateTimeRange(_diwali, _diwali), new DateTimeRange(_christmasDay, _christmasDay), false },
            new object[] { new DateTimeRange(_christmasDay, _newYearsDay), new DateTimeRange(_christmasDay, _newYearsDay), true },
            new object[] { new DateTimeRange(_christmasDay, _christmasDay), new DateTimeRange(_christmasDay, _christmasDay), true },
        };

        public readonly static IEnumerable<object[]> HashCodeTestCases = new[]{
            new object[] { new DateTimeRange(), new DateTimeRange(), true },
            new object[] { new DateTimeRange(_christmasDay, _newYearsDay), new DateTimeRange(), false },
            new object[] { new DateTimeRange(), new DateTimeRange(_christmasDay, _newYearsDay),  false },
            new object[] { new DateTimeRange(_diwali, _christmasDay), new DateTimeRange(_christmasDay, _newYearsDay), false },
            new object[] { new DateTimeRange(_diwali, _diwali), new DateTimeRange(_christmasDay, _christmasDay), false },
            new object[] { new DateTimeRange(_christmasDay, _newYearsDay), new DateTimeRange(_christmasDay, _newYearsDay), true },
            new object[] { new DateTimeRange(_christmasDay, _christmasDay), new DateTimeRange(_christmasDay, _christmasDay), true },

            new object[] {
                new DateTimeRange(new DateTime(2018, 12, 31, 13, 00, 00), new DateTime(2018, 12, 31, 14, 00, 00)),
                new DateTimeRange(new DateTime(2018, 12, 31, 14, 00, 00), new DateTime(2018, 12, 31, 15, 00, 00)),
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 12, 31, 13, 00, 00), new DateTime(2018, 12, 31, 14, 00, 00)),
                new DateTimeRange(new DateTime(2018, 12, 30, 13, 00, 00), new DateTime(2018, 12, 31, 14, 00, 00)),
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 12, 31, 13, 00, 00), new DateTime(2018, 12, 31, 14, 00, 00)),
                new DateTimeRange(new DateTime(2018, 12, 31, 13, 00, 00), new DateTime(2018, 12, 31, 14, 00, 00)),
                true
            },
        };

        public readonly static IEnumerable<object[]> OverlayTestCases = new[]{
            new object[] {
                new DateTimeRange(), // ||
                new DateTimeRange(), // ||
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                DateTimeRange.CompareStrategy.IgnoreEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                new DateTimeRange(new DateTime(2018, 2, 2), new DateTime(2018, 3, 1)), //           |________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 2), new DateTime(2018, 3, 1)), //           |________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                DateTimeRange.CompareStrategy.IncludeEdges,
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1, 0, 0, 0), new DateTime(2018, 1, 1, 3, 0, 0)), // |_______|
                new DateTimeRange(new DateTime(2018, 1, 1, 3, 1, 0), new DateTime(2018, 1, 1, 4, 0 ,0)), //           |________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1, 0, 0, 0), new DateTime(2018, 1, 1, 3, 0, 0)), // |_______|
                new DateTimeRange(new DateTime(2018, 1, 1, 3, 0, 0), new DateTime(2018, 1, 1, 4, 0 ,0)), //         |________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1, 0, 0, 0), new DateTime(2018, 1, 1, 3, 0, 0)), // |_______|
                new DateTimeRange(new DateTime(2018, 1, 1, 3, 0, 0), new DateTime(2018, 1, 1, 4, 0 ,0)), //         |________|
                DateTimeRange.CompareStrategy.IgnoreEdges,
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 3, 1)), //         |________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 3, 1)), //         |________|
                DateTimeRange.CompareStrategy.IgnoreEdges,
                false
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 3, 1)), // |__________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 3, 1)), // |__________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 2, 1)), // |_______|
                DateTimeRange.CompareStrategy.IgnoreEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 4, 1)), //      |________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 3, 1)), // |_______|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 4, 1)), //      |________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 4, 1)), // |_____________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 4, 1)), //      |________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 4, 1)), // |_____________|
                DateTimeRange.CompareStrategy.IgnoreEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 4, 1)), //      |________|
                new DateTimeRange(new DateTime(2018, 1, 1), new DateTime(2018, 6, 1)), // |__________________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 4, 1)), //      |________|
                new DateTimeRange(new DateTime(2018, 3, 1), new DateTime(2018, 6, 1)), //           |________|
                DateTimeRange.CompareStrategy.IncludeEdges,
                true
            }
        };

        public static readonly IEnumerable<object[]> IteratorTestCases = new[]{
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 2, 1)),
                1
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 2, 2)),
                2
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2018, 3, 1)),
                29
            },
            new object[] {
                new DateTimeRange(new DateTime(2018, 2, 1), new DateTime(2019, 2, 1)),
                366
            },
        };
    }
}
