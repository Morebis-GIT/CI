using System;
using System.Collections.Generic;
using AutoFixture;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.tests.ValidationTests.Runs.Data
{
    public sealed class RunsValidationsTestData
    {
        private static readonly Fixture _fixture = new Fixture();
        private static readonly TenantSettings _tenantSettingsA = _fixture.Build<TenantSettings>()
                    .With(o => o.MidnightStartTime, "240000")
                    .With(o => o.MidnightEndTime, "995959")
                    .With(o => o.PeakStartTime, "180000")
                    .With(o => o.PeakEndTime, "220000")
                    .Create();

        private static readonly TimeSpan[][] _dateTimes = {
            new TimeSpan[] { new TimeSpan(0, 6, 0, 0), new TimeSpan(1, 5, 59, 59) }, // 06:00:00 - 1 day 05:59:59
            new TimeSpan[] { new TimeSpan(0, 6, 0, 0), new TimeSpan(0, 23, 59, 59) }, // 06:00:00 - 23:59:59
            new TimeSpan[] { new TimeSpan(0, 12, 0, 0), new TimeSpan(0, 15, 59, 59) }, // 12:00:00 - 15:59:59
            new TimeSpan[] { new TimeSpan(0, 20, 0, 0), new TimeSpan(1, 5, 59, 59) }, // 20:00:00 - 1 day 05:59:59
            new TimeSpan[] { new TimeSpan(1, 0, 0, 0), new TimeSpan(1, 3, 59, 59) }, // 1 day 00:00:00 - 1 day 03:59:59
            new TimeSpan[] { new TimeSpan(1, 4, 0, 0), new TimeSpan(1, 5, 59, 59) } // 1 day 04:00:00 - 1 day 05:59:59
        };

        private const string PassName = "Test Pass";

        private const string ValidatePassSalesAreaPrioritiesDaypartErrorMessage =
            "Pass: {0}, SalesAreaPriorities {1} daypart is not within run StartTime/EndTime";

        private const string ValidatePassSalesAreaPriorityTimesErrorMessage =
            "Pass: Test Pass, SalesAreaPriorities StartTime/EndTime not within run StartTime/EndTime";

        // assume data transformation was applied that does not allow end time to be less than start time
        // Fields: runStartTime; runEndTime; tenantSettings; passName expectedResult;
        public static IEnumerable<object[]> ValidatePassSalesAreaPrioritiesDaypartTestCases =>

                    new List<object[]>{
                    // Off-Peak pass period potentially consists of 3 time segments:
                    // 06:00-peak, peak-midnight and midnight-05:59
                    // by default midnight time is 00:00:00 - 05:59:59, so off-peak is 06:00 - 23:59:59

                    // 06:00:00 - 1 day 05:59:59 SUCCESS
                    new object[] { Dayparts.OffPeak, _dateTimes[0][0], _dateTimes[0][1], _tenantSettingsA, PassName, String.Empty },
                    // 00:06:00 - 23:59:59 SUCCESS
                    new object[] { Dayparts.OffPeak, _dateTimes[1][0], _dateTimes[1][1], _tenantSettingsA, PassName, String.Empty },
                    // 12:00:00 - 15:59:59 FAIL
                    new object[] { Dayparts.OffPeak, _dateTimes[2][0], _dateTimes[2][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.OffPeak) },
                    // 20:00:00 - 1 day 05:59:59 FAIL
                    new object[] { Dayparts.OffPeak, _dateTimes[3][0], _dateTimes[3][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.OffPeak) },
                    // 1 day 00:00:00 - 1 day 03:59:59 FAIL
                    new object[] { Dayparts.OffPeak, _dateTimes[4][0], _dateTimes[4][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.OffPeak) },
                    // 1 day 04:00:00 - 1 day 05:59:59 FAIL
                    new object[] { Dayparts.OffPeak, _dateTimes[5][0], _dateTimes[5][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.OffPeak) },

                    // 06:00:00 - 1 day 05:59:59 SUCCESS
                    new object[] { Dayparts.Peak, _dateTimes[0][0], _dateTimes[0][1], _tenantSettingsA, PassName, String.Empty  },
                    // 06:00:00 - 23:59:59 SUCCESS
                    new object[] { Dayparts.Peak, _dateTimes[1][0], _dateTimes[1][1], _tenantSettingsA, PassName, String.Empty },
                    // 12:00:00 - 15:59:59 FAIL
                    new object[] { Dayparts.Peak, _dateTimes[2][0], _dateTimes[2][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.Peak) },
                    // 20:00:00 - 1 day 05:59:59 FAIL
                    new object[] { Dayparts.Peak, _dateTimes[3][0], _dateTimes[3][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.Peak) },
                    // 1 day 00:00:00 - 1 day 03:59:59 FAIL
                    new object[] { Dayparts.Peak, _dateTimes[4][0], _dateTimes[4][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.Peak) },
                    // 1 day 04:00:00 - 1 day 05:59:59 FAIL
                    new object[] { Dayparts.Peak, _dateTimes[5][0], _dateTimes[5][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.Peak) },

                    // 06:00:00 - 1 day 05:59:59 SUCCESS
                    new object[] { Dayparts.MidnightToDawn, _dateTimes[0][0], _dateTimes[0][1], _tenantSettingsA, PassName, String.Empty  },
                    // 06:00:00 - 23:59:59 FAIL
                    new object[] { Dayparts.MidnightToDawn, _dateTimes[1][0], _dateTimes[1][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.MidnightToDawn) },
                    // 12:00:00 - 15:59:59 FAIL
                    new object[] { Dayparts.MidnightToDawn, _dateTimes[2][0], _dateTimes[2][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.MidnightToDawn) },
                    // 20:00:00 - 1 day 05:59:59 SUCCESS
                    new object[] { Dayparts.MidnightToDawn, _dateTimes[3][0], _dateTimes[3][1], _tenantSettingsA, PassName, String.Empty },
                    // 1 day 00:00:00 - 1 day 03:59:59 FAIL
                    new object[] { Dayparts.MidnightToDawn, _dateTimes[4][0], _dateTimes[4][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.MidnightToDawn) },
                    // 1 day 04:00:00 - 1 day 05:59:59 FAIL
                    new object[] { Dayparts.MidnightToDawn, _dateTimes[5][0], _dateTimes[5][1], _tenantSettingsA, PassName,
                        String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.MidnightToDawn) },
                    };

        // Fields: runStartTime runEndTime expectedResult
        public static IEnumerable<object[]> ValidatePassSalesAreaPrioritiesDayPartTrueTestCases =>
            new List<object[]>{
                new object[] {new TimeSpan(6,0,0), new TimeSpan(5,59,59), String.Empty},
                new object[] {new TimeSpan(6,0,0), new TimeSpan(23,59,59),
                    String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.MidnightToDawn) + Environment.NewLine + Environment.NewLine},
                new object[] {new TimeSpan(18,0,0), new TimeSpan(5,59,59),
                    String.Format(ValidatePassSalesAreaPrioritiesDaypartErrorMessage, PassName, Dayparts.OffPeak) + Environment.NewLine + Environment.NewLine}
            };

        // Fields: runStartTime runEndTime passStartTime, passEndTime, expectedResult
        public static IEnumerable<object[]> ValidatePassSalesAreaPrioritiesDayPartFalseTestCases =>
            new List<object[]>{
                // run 06:00:00 - 05:59:59, pass 06:00:00 - 05:59:59 SUCCESS
                new object[]
                {
                    new TimeSpan(6,0,0), new TimeSpan(5,59,59),
                    new TimeSpan(6,0,0), new TimeSpan(5,59,59),  
                    String.Empty
                },
                // run 10:00:00 - 20:59:59, pass 12:00:00 - 18:59:59 SUCCESS
                new object[]
                {
                    new TimeSpan(10,0,0), new TimeSpan(20,59,59),
                    new TimeSpan(12,0,0), new TimeSpan(18, 59, 59),
                    String.Empty
                },
                // run 00:00:00 - 05:59:59, pass 02:00:00 - 04:59:59 SUCCESS
                new object[]
                {
                    new TimeSpan(0, 0, 0), new TimeSpan(5, 59,59),
                    new TimeSpan(2,0,0), new TimeSpan(4,59,59),
                    String.Empty
                },
                // run 12:00:00 - 21:59:59, pass 10:00:00 - 14:59:59 FAIL
                new object[]
                {
                    new TimeSpan(12, 0, 0), new TimeSpan(21, 59,59),
                    new TimeSpan(10,0,0), new TimeSpan(14,59,59),
                    ValidatePassSalesAreaPriorityTimesErrorMessage + Environment.NewLine
                },
                // run 08:00:00 - 11:59:59, pass 10:00:00 - 15:59:59 FAIL
                new object[]
                {
                    new TimeSpan(8, 0, 0), new TimeSpan(11, 59,59),
                    new TimeSpan(10,0,0), new TimeSpan(15,59,59),
                    ValidatePassSalesAreaPriorityTimesErrorMessage + Environment.NewLine
                },
                // run 12:00:00 - 17:59:59, pass 10:00:00 - 21:59:59 FAIL
                new object[]
                {
                    new TimeSpan(8, 0, 0), new TimeSpan(11, 59,59),
                    new TimeSpan(10,0,0), new TimeSpan(14,59,59),
                    ValidatePassSalesAreaPriorityTimesErrorMessage + Environment.NewLine
                },
                // run 24:00:00 - 05:59:59, pass 06:00:00 - 05:59:59 FAIL
                new object[]
                {
                    new TimeSpan(24, 0, 0), new TimeSpan(5, 59,59),
                    new TimeSpan(6,0,0), new TimeSpan(5,59,59),
                    ValidatePassSalesAreaPriorityTimesErrorMessage + Environment.NewLine
                },
                // run 01:00:00 - 03:59:59, pass 22:00:00 - 23:59:59 FAIL
                new object[]
                {
                    new TimeSpan(1, 0, 0), new TimeSpan(3, 59,59),
                    new TimeSpan(22,0,0), new TimeSpan(23,59,59),
                    ValidatePassSalesAreaPriorityTimesErrorMessage + Environment.NewLine
                },
                // run 00:00:00 - 23:59:59, pass 00:00:00 - 23:59:59 SUCCESS
                new object[]
                {
                    new TimeSpan(0,0,0), new TimeSpan(23, 59, 59),
                    new TimeSpan(0,0,0), new TimeSpan(23, 59, 59),
                    String.Empty
                },
                // run 00:00:00 - 23:59:59, pass 02:00:00 - 23:59:59 SUCCESS
                new object[]
                {
                    new TimeSpan(0,0,0), new TimeSpan(23, 59, 59),
                    new TimeSpan(2,0,0), new TimeSpan(23, 59, 59),
                    String.Empty
                }
            };
    }
}
