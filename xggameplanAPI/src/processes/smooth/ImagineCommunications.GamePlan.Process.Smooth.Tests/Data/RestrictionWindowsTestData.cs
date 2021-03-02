using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Data
{
    public sealed class RestrictionWindowsTestData
    {
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
