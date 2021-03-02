using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Data;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Restriction window")]
    public static class RestrictionWindowTests
    {
        private static readonly DateTime _breakScheduledFor = new DateTime(2018, 09, 20, 18, 00, 00);

        [Theory]
        [MemberData(
            nameof(RestrictionWindowsTestData.RestrictionWindowTestCases),
            MemberType = typeof(RestrictionWindowsTestData)
            )]
        public static void Check_the_restriction_window_applies_to_a_break(
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
            _ = result.Should().Be(expect, becauseArgs: null);
        }
    }
}
