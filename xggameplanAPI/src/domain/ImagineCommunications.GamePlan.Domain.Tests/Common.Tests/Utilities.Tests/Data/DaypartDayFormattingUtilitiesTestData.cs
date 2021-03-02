using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Utilities.Tests.Data
{
    public sealed class DaypartDayFormattingUtilitiesTestData
    {
        private static readonly string _monday = DayOfWeek.Monday.ToString();
        private static readonly string _tuesday = DayOfWeek.Tuesday.ToString();
        private static readonly string _wednesday = DayOfWeek.Wednesday.ToString();
        private static readonly string _thursday = DayOfWeek.Thursday.ToString();
        private static readonly string _friday = DayOfWeek.Friday.ToString();
        private static readonly string _saturday = DayOfWeek.Saturday.ToString();
        private static readonly string _sunday = DayOfWeek.Sunday.ToString();

        private static readonly string[] _sundayWeek = new string[] { _sunday, _monday, _tuesday, _wednesday, _thursday, _friday, _saturday };
        private static readonly string[] _wednesdayWeek = new string[] { _wednesday, _thursday, _friday, _saturday, _sunday, _monday, _tuesday };

        public static string[] TestWeek { get { return _wednesdayWeek; } }

        public static IEnumerable<object[]> WeekDaysWithCustomStartTestCases =>
                new List<object[]>{
            // Fields:
            // start week day;
            // ordered week days;

                new object[] { DayOfWeek.Monday, new string[] { _monday, _tuesday, _wednesday, _thursday, _friday, _saturday, _sunday } },
                new object[] { DayOfWeek.Tuesday, new string[] { _tuesday, _wednesday, _thursday, _friday, _saturday, _sunday, _monday } },
                new object[] { DayOfWeek.Wednesday, new string[] { _wednesday, _thursday, _friday, _saturday, _sunday, _monday, _tuesday } },
                new object[] { DayOfWeek.Thursday, new string[] { _thursday, _friday, _saturday, _sunday, _monday, _tuesday, _wednesday } },
                new object[] { DayOfWeek.Friday, new string[] { _friday, _saturday, _sunday, _monday, _tuesday, _wednesday, _thursday } },
                new object[] { DayOfWeek.Saturday, new string[] { _saturday, _sunday, _monday, _tuesday, _wednesday, _thursday, _friday } },
                new object[] { DayOfWeek.Sunday, new string[] { _sunday, _monday, _tuesday, _wednesday, _thursday, _friday, _saturday } },
                };

        public static IEnumerable<object[]> FormatWeekDaysTestCases =>
            new List<object[]>
        {
                // Fields:
                // week days;
                // daypart days;
                // formated daypart days;

                new object[] { _sundayWeek, "YYYYYYY", DayOfWeek.Sunday, "S M T W T F S" },
                new object[] { _sundayWeek, "YYNNYYY", DayOfWeek.Sunday, "S M T - - F S" },
                new object[] { _sundayWeek, "YNYYNYN", DayOfWeek.Sunday, "- M - W T - S" },
                new object[] { _wednesdayWeek, "YYYYYYY", DayOfWeek.Wednesday, "W T F S S M T"},
                new object[] { _wednesdayWeek, "YYNNYYY", DayOfWeek.Wednesday, "- - F S S M T"},
                new object[] { _wednesdayWeek, "YNYYNYN", DayOfWeek.Wednesday, "W T - S - M -"},
            };
    }
}
