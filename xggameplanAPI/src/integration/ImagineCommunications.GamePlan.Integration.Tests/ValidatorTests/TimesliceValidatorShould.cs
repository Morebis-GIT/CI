using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared;
using Xunit;

namespace ImagineCommunications.GamePlan.Integration.Tests.ValidatorTests
{
    public class TimesliceValidatorShould
    {
        private static readonly List<string> _fullDowPattern = new List<string>
        {
            "Sun",
            "Mon",
            "Tue",
            "Wed",
            "Thu",
            "Fri",
            "Sat"
        };

        [Theory]
        [MemberData(nameof(ValidData))]
        public void TimesliceModel_Valid(string fromTime, string toTime, List<string> dowPattern)
        {
            var validator = new TimesliceValidator();
            var res = validator.Validate(new Timeslice(fromTime, toTime, dowPattern));
            Assert.True(res.IsValid,
                $"Invalid {nameof(Timeslice)} model. {nameof(Timeslice.FromTime)}: {fromTime}, {nameof(Timeslice.ToTime)}: {toTime}, {nameof(Timeslice.DowPattern)}: {string.Join("|", dowPattern.AsEnumerable() ?? Array.Empty<string>())}");
        }

        [Theory]
        [MemberData(nameof(InvalidData))]
        public void TimesliceModel_Invalid(string fromTime, string toTime, List<string> dowPattern, int errorCount)
        {
            var validator = new TimesliceValidator();
            var res = validator.Validate(new Timeslice(fromTime, toTime, dowPattern));
            Assert.False(res.IsValid);
            Assert.Equal(res.Errors.Count, errorCount);
        }

        public static IEnumerable<object[]> ValidData => new[]
        {
            new object[] { "4:05", "8:03", _fullDowPattern },
            new object[] { "5:54:20", "9:13:45", _fullDowPattern },
            new object[] { "09:15", "21:33", _fullDowPattern },
            new object[] { "12:16:31", "22:21:00", _fullDowPattern },
            new object[] { "00:00:00", "23:59:59", _fullDowPattern }
        };

        public static IEnumerable<object[]> InvalidData => new[]
        {
            new object[] { string.Empty, string.Empty, new List<string>(), 5 },
            new object[] { "24:05", "8:99", _fullDowPattern, 2 },
            new object[] { "23:54:99", "9:03:45", new List<string>(), 2 },
            new object[] { "09:15", "29:33", _fullDowPattern, 1 },
            new object[] { "12:16:31", "23:63:59", _fullDowPattern, 1 },
            new object[] { "15:40:00", "05:59:59", new List<string> { "a1", "a2", "a3" }, 3 }
        };
    }
}
