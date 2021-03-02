using System;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Domain.Tests.Types.Ranges.Data
{
    public sealed class DateTimeRangeTestData
    {
        private readonly static DateTime EmptyDate = new DateTime();
        private readonly static DateTime Halloween = new DateTime(2018, 10, 31);
        private readonly static DateTime Diwali = new DateTime(2018, 11, 7);
        private readonly static DateTime ChristmasDay = new DateTime(2018, 12, 25);
        private readonly static DateTime NewYearsDay = new DateTime(2019, 1, 1);

        public readonly static object[] ExplicitDateTimeConversionTestCases = {
            new object[] { EmptyDate, EmptyDate, new DateTimeRange(EmptyDate, EmptyDate) },
            new object[] { ChristmasDay, EmptyDate, new DateTimeRange(ChristmasDay, EmptyDate) },
            new object[] { EmptyDate, ChristmasDay, new DateTimeRange(EmptyDate, ChristmasDay) },
            new object[] { ChristmasDay, NewYearsDay, new DateTimeRange(ChristmasDay, NewYearsDay) }
        };

        public readonly static object[] ContainsTestCases = {
            new object[] { EmptyDate, EmptyDate, EmptyDate, true },
            new object[] { ChristmasDay, EmptyDate, EmptyDate, false },
            new object[] { EmptyDate, ChristmasDay, EmptyDate, true },
            new object[] { EmptyDate, EmptyDate, ChristmasDay, false },

            new object[] { ChristmasDay, ChristmasDay, Diwali, false },
            new object[] { ChristmasDay, ChristmasDay, ChristmasDay, true },
            new object[] { ChristmasDay, ChristmasDay, NewYearsDay, false },

            new object[] { Diwali, ChristmasDay, Halloween, false },
            new object[] { Diwali, ChristmasDay, Diwali, true },
            new object[] { Diwali, ChristmasDay, ChristmasDay, true },
            new object[] { Diwali, ChristmasDay, NewYearsDay, false },

            new object[] { ChristmasDay, NewYearsDay, ChristmasDay, true },
            new object[] { NewYearsDay, NewYearsDay, ChristmasDay, false },

            // End date is before start date
            new object[] { NewYearsDay, ChristmasDay, ChristmasDay, false },

            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 6), false },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 5), new DateTime(2018, 12, 25, 12, 59, 0), false },

            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 0), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 0), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 5), new DateTime(2018, 12, 25, 13, 0, 6), true },
            new object[] { new DateTime(2018, 12, 25, 13, 0, 0), new DateTime(2018, 12, 26, 13, 0, 5), new DateTime(2018, 12, 25, 12, 59, 0), false },
        };

        public readonly static object[] EqualsTestCases = {
            new object[] { new DateTimeRange(), new DateTimeRange(), true },
            new object[] { new DateTimeRange(ChristmasDay, NewYearsDay), new DateTimeRange(), false },
            new object[] { new DateTimeRange(), new DateTimeRange(ChristmasDay, NewYearsDay),  false },
            new object[] { new DateTimeRange(Diwali, ChristmasDay), new DateTimeRange(ChristmasDay, NewYearsDay), false },
            new object[] { new DateTimeRange(Diwali, Diwali), new DateTimeRange(ChristmasDay, ChristmasDay), false },
            new object[] { new DateTimeRange(ChristmasDay, NewYearsDay), new DateTimeRange(ChristmasDay, NewYearsDay), true },
            new object[] { new DateTimeRange(ChristmasDay, ChristmasDay), new DateTimeRange(ChristmasDay, ChristmasDay), true },
        };

        public readonly static object[] HashCodeTestCases = {
            new object[] { new DateTimeRange(), new DateTimeRange(), true },
            new object[] { new DateTimeRange(ChristmasDay, NewYearsDay), new DateTimeRange(), false },
            new object[] { new DateTimeRange(), new DateTimeRange(ChristmasDay, NewYearsDay),  false },
            new object[] { new DateTimeRange(Diwali, ChristmasDay), new DateTimeRange(ChristmasDay, NewYearsDay), false },
            new object[] { new DateTimeRange(Diwali, Diwali), new DateTimeRange(ChristmasDay, ChristmasDay), false },
            new object[] { new DateTimeRange(ChristmasDay, NewYearsDay), new DateTimeRange(ChristmasDay, NewYearsDay), true },
            new object[] { new DateTimeRange(ChristmasDay, ChristmasDay), new DateTimeRange(ChristmasDay, ChristmasDay), true },

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

        public readonly static object[] OverlayTestCases = {
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

        internal static readonly object[] IteratorTestCases =
        {
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
