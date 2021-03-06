﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Trait("Smooth", "Restriction window")]
    public class RestrictionWindowTests
    {
        private static readonly DateTime _breakScheduledFor =
            new DateTime(2018, 09, 20, 18, 00, 00);
        private readonly ITestOutputHelper _output;

        public RestrictionWindowTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [MemberData(nameof(RestrictionWindowTestCases))]
        public void Check_the_restriction_window_applies_to_a_break(
            DateTime startDate,
            DateTime? endDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            bool expect
            )
        {
            // Arrange
            var restrictionWindow = new RestrictionWindow(
                (startDate, endDate),
                (startTime, endTime)
                );

            // Act
            bool result = restrictionWindow.Contains(_breakScheduledFor);

            // Assert
            if (result != expect)
            {
                _output.WriteLine("----TEST FAILURE--------------");
                _output.WriteLine($"break: {_breakScheduledFor.ToString()}");
                _output.WriteLine($"start date: {startDate.ToString()}");
                _output.WriteLine($"start time: {startTime?.ToString() ?? "null"}");
                _output.WriteLine($"end date: {endDate?.ToString() ?? "null"}");
                _output.WriteLine($"end time: {endTime?.ToString() ?? "null"}");
                _output.WriteLine($"expect: {expect.ToString()}");
                _output.WriteLine($"result: {result.ToString()}");
                _output.WriteLine("----End--------------");
            }

            _ = result.Should().Be(expect, null);
        }

        /// <summary>
        /// Test values for a restriction window.
        /// </summary>
        public static IEnumerable<object[]> RestrictionWindowTestCases =>
            new List<object[]>{
            /*
            * NOTE: These values ONLY work if the break schedule is 20 Sept 2018 at 18:00
            */

            // Fields:
            // start date; end date; start time; end time; restriction window contains break?

            // Somehow a null gets in... (the original code checked for this)
            new object[] { null, null, null, null, false },

            new object[] { new DateTime(2018, 09, 19), null, null, null, true },
            new object[] { new DateTime(2018, 09, 20), null, null, null, true },
            new object[] { new DateTime(2018, 09, 21), null, null, null, false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), null, null, false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), null, null, true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), null, null, true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), null, null, true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), null, null, true },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), null, null, false },

            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(17, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(18, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(17, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(18, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(17, 00, 00), null, false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(18, 00, 00), null, false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), null, new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), null, new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), null, new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(17, 00, 00), null, false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(18, 00, 00), null, false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(17, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(18, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(17, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(18, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), null, false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), null, false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), null, true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), null, false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), new TimeSpan(19, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(16, 00, 00), new TimeSpan(17, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(17, 00, 00), new TimeSpan(18, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(18, 00, 00), new TimeSpan(19, 00, 00), false },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(19, 00, 00), new TimeSpan(20, 00, 00), false },

            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 19), new TimeSpan(16, 00, 00), new TimeSpan(20, 00, 00), false },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 20), new TimeSpan(16, 00, 00), new TimeSpan(20, 00, 00), true },
            new object[] { new DateTime(2018, 09, 19), new DateTime(2018, 09, 21), new TimeSpan(16, 00, 00), new TimeSpan(20, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 20), new TimeSpan(16, 00, 00), new TimeSpan(20, 00, 00), true },
            new object[] { new DateTime(2018, 09, 20), new DateTime(2018, 09, 21), new TimeSpan(16, 00, 00), new TimeSpan(20, 00, 00), true },
            new object[] { new DateTime(2018, 09, 21), new DateTime(2018, 09, 21), new TimeSpan(16, 00, 00), new TimeSpan(20, 00, 00), false },
        };
    }
}
