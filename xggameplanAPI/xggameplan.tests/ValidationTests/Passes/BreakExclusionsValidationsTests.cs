using System;
using System.Collections.Generic;
using System.Globalization;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using NodaTime;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations.Passes;

namespace xggameplan.tests.ValidationTests.Passes
{
    [TestFixture(Category = "Validations :: BreakExclusions")]
    internal static class BreakExclusionsValidationsTests
    {
        private const string SalesAreaDescriptionFormat = "[Name: '{0} ({1})', StartTime: '{2}', Duration(hours): '{3}']";
        private const string EndDateBeforeStartDateMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndDate = {1}' must be after  'StartDate = {2}'";
        private const string StartimeIsAfterEndTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndTime = {1}' must be after 'StartTime = {2}'";
        private const string EndTimeIsAfterBroadcastEndTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndTime = {1}' must be before 'Broadcast.EndTime = {2}'";
        private const string StartTimeIsAfterBroadcastEndTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'StartTime = {1}' must be before 'Broadcast.EndTime = {2}'";
        private const string StartTimeIsBeforeBroadcastStartTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'StartTime = {1}' must be equal or after 'Broadcast.StartTime = {2}'";
        private const string EndTimeIsBeforeBroadcastStartTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndTime = {1}' must be after 'Broadcast.StartTime = {2}'";

        [Test]
        [Description(@"Given valid break exclusions are provided
                       When checking the break exclusions are valid
                       Then validation should pass"),]
        // ******* Successful Validation Test Cases ******* Broadcat start
        // Duration startTime endTime
        [TestCase("06:00:00", "1.00:00:00", "07:00:00", "10:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "07:00:00", "17:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "07:00:00", "05:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "23:00:00", "04:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "03:00:00", "04:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "06:00:00", "05:59:59")]
        [TestCase("06:00:00", "1.00:00:00", "06:00:00", "16:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "16:00:00", "05:59:59")]
        [TestCase("06:00:00", "1.00:00:00", "--null--", "09:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "09:00:00", "--null--")]
        [TestCase("08:00:00", "02:00:00", "08:00:00", "09:00:00")]
        [TestCase("08:00:00", "02:00:00", "09:00:00", "09:59:59")]
        [TestCase("08:00:00", "02:00:00", "08:00:00", "09:59:59")]
        [TestCase("08:00:00", "02:00:00", "08:30:00", "09:30:00")]
        [TestCase("06:00:00", "1.06:00:00", "06:00:00", "16:00:00")]
        [TestCase("06:00:00", "1.06:00:00", "16:00:00", "11:59:59")]
        [TestCase("06:00:00", "1.06:00:00", "06:00:00", "11:59:59")]
        [TestCase("06:00:00", "1.06:00:00", "07:00:00", "08:00:00")]
        public static void BreakExclusionsDateTimeRangeIsValid_Passes_WithValidData(string broadcastStartTime
            , string broadcastDuration, string breakExclusionStartTime, string breakExclusionEndTime)
        {
            var salesAreaName = "sa1";
            var salesAreas = GetSalesAreas(salesAreaName, broadcastStartTime, broadcastDuration);
            var breakExclusionsList = GetBreakExclusionsList(
                salesAreaName,
                DateTime.UtcNow.Date,
                DateTime.UtcNow.Date,
                breakExclusionStartTime, breakExclusionEndTime);
            List<string> errorMessage;
            Assert.IsTrue(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        [Description(@"Given invalid break exclusions are provided
                       When checking the break exclusions are valid
                       Then validation should fail"),]
        // ******** Failing Validation Test Cases ******** Broadcat start
        // Duration startTime endTime
        [TestCase("06:00:00", "1.00:00:00", "03:00:00", "07:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "03:00:00", "02:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "08:00:00", "07:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "07:00:00", "07:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "06:00:00", "06:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "22:00:00", "21:00:00")]
        [TestCase("06:00:00", "1.00:00:00", "02:00:00", "21:00:00")]
        [TestCase("08:00:00", "0.02:00:00", "07:00:00", "10:00:00")]
        [TestCase("08:00:00", "0.02:00:00", "07:00:00", "10:30:00")]
        [TestCase("08:00:00", "0.02:00:00", "08:30:00", "10:00:00")]
        [TestCase("08:00:00", "0.02:00:00", "10:30:00", "09:00:00")]
        [TestCase("06:00:00", "1.06:00:00", "05:00:00", "14:00:00")]
        [TestCase("06:00:00", "1.06:00:00", "14:00:00", "13:00:00")]
        [TestCase("06:00:00", "1.06:00:00", "23:00:00", "13:00:00")]

        public static void BreakExclusionsDateTimeRangeIsValid_Failes_WithInvalidData(string broadcastStartTime
            , string broadcastDuration, string breakExclusionStartTime, string breakExclusionEndTime)
        {
            var salesAreaName = "sa1";
            var salesAreas = GetSalesAreas(salesAreaName, broadcastStartTime, broadcastDuration);
            var breakExclusionsList = GetBreakExclusionsList(salesAreaName,
                DateTime.UtcNow.Date,
                DateTime.UtcNow.Date,
                breakExclusionStartTime,
                breakExclusionEndTime);
            List<string> errorMessage;
            Assert.IsFalse(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        [Description(@"Given BreakExclusion is null
                       When break exclusions are validated
                       Then validation should Pass"),]
        public static void BreakExclusionsDateTimeRangeIsValid_Passes_WithNullBreakExclusion()
        {
            var salesAreaName = "sa1";
            var salesAreas = GetSalesAreas(salesAreaName, "06:00:00", "1.00:00:00");
            List<BreakExclusionModel> breakExclusionsList = null;
            List<string> errorMessage;
            Assert.IsTrue(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        [Description(@"Given SalesArea is null
                       When break exclusions are validated
                       Then validation should Pass"),]
        public static void BreakExclusionsDateTimeRangeIsValid_Passes_WithNullSalesArea()
        {
            var salesAreaName = "sa1";
            List<SalesArea> salesAreas = null;
            List<BreakExclusionModel> breakExclusionsList = GetBreakExclusionsList(salesAreaName,
                DateTime.UtcNow.Date, DateTime.UtcNow.Date, "03:00:00", "02:00:00");
            List<string> errorMessage;
            Assert.IsTrue(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        [Description(@"Given SalesArea is not a match
                       When break exclusions are validated
                       Then validation should Pass"),]
        public static void BreakExclusionsDateTimeRangeIsValid_Passes_WithNotMachingSalesArea()
        {
            List<SalesArea> salesAreas = GetSalesAreas("sa1", "06:00:00", "1.00:00:00");
            List<BreakExclusionModel> breakExclusionsList = GetBreakExclusionsList("sa2",
                DateTime.UtcNow.Date, DateTime.UtcNow.Date, "03:00:00", "02:00:00");
            List<string> errorMessage;
            Assert.IsTrue(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        [Description(@"Given break exclusions startDate is after the endDate
                       When break exclusions are validated
                       Then validation should not Pass"),]
        public static void BreakExclusionsDateTimeRangeIsValid_Fails_WithBreakExclusionsStartDateAfterTheEndDate()
        {
            var salesAreaName = "sa1";
            List<SalesArea> salesAreas = GetSalesAreas(salesAreaName, "06:00:00", "03:00:00");
            List<BreakExclusionModel> breakExclusionsList = GetBreakExclusionsList(salesAreaName,
                DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date, "07:00:00", "08:00:00");
            List<string> errorMessage;
            Assert.IsFalse(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        [Description(@"Given break exclusions End Date is Before Start Date
                       When break exclusions are validated
                       Then validation should return proper error message"),]
        public static void BreakExclusionsDateRangeIsValid_FailsWithProperErrorMessage()
        {
            var salesAreaName = "sa1";
            List<SalesArea> salesAreas = GetSalesAreas(salesAreaName, "06:00:00", "3:00:00");
            List<BreakExclusionModel> breakExclusionsList = new List<BreakExclusionModel>();
            breakExclusionsList.Add(GetBreakExclusion(salesAreaName, DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date, "07:00:00", "07:30:00"));
            List<string> errorMessage;
            var expectedMessage = EndDateBeforeStartDateMessage(salesAreas[0], breakExclusionsList[0]);
            Assert.IsFalse(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessage));
            Assert.AreEqual(errorMessage.Count, 1);
            Assert.AreEqual(errorMessage[0], expectedMessage);
        }

        [Test]
        [Description(@"Given break exclusions time is in valid
                       When break exclusions are validated
                       Then validation should return proper error message"),]
        // ******* Failing Validation Test Cases for Time ******* salesarea
        // start Duration be.startTime be.endTime
        [TestCase("S01", "08:00:00", "02:00:00", "06:00:00", "07:00:00")]
        [TestCase("S02", "08:00:00", "02:00:00", "07:00:00", "08:00:00")]
        [TestCase("S03", "08:00:00", "02:00:00", "07:00:00", "09:00:00")]
        [TestCase("S04", "08:00:00", "02:00:00", "09:00:00", "11:00:00")]
        [TestCase("S05", "08:00:00", "02:00:00", "10:00:00", "11:00:00")]
        [TestCase("S06", "08:00:00", "02:00:00", "11:00:00", "12:00:00")]
        [TestCase("S07", "08:00:00", "02:00:00", "07:00:00", "09:59:59")]
        [TestCase("S08", "08:00:00", "02:00:00", "08:00:00", "11:00:00")]
        [TestCase("S09", "08:00:00", "02:00:00", "06:00:00", "12:00:00")]
        [TestCase("S10", "06:00:00", "1.00:00:00", "05:00:00", "16:00:00")]
        [TestCase("S11", "06:00:00", "1.00:00:00", "16:00:00", "10:00:00")]
        [TestCase("S12", "06:00:00", "1.06:00:00", "04:00:00", "16:00:00")]
        [TestCase("S13", "06:00:00", "1.06:00:00", "16:00:00", "14:00:00")]
        public static void BreakExclusionsTimeRangeIsValid_FailsWithProperErrorMessage(string salesAreaName,
            string broadcastStartTime, string broadcastDuration, string breakExclusionStartTime, string breakExclusionEndTime)
        {
            var salesArea = GetSalesArea(salesAreaName, broadcastStartTime, broadcastDuration);
            var breakExclusion = GetBreakExclusion(salesAreaName, DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1), breakExclusionStartTime, breakExclusionEndTime);
            var expectedMessages = new Dictionary<string, List<string>>() {
                {
                    "S01",
                    new List<string>(){
                        StartTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                        EndTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S02",
                    new List<string>(){
                        StartTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                        EndTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S03",
                    new List<string>(){
                        StartTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S04",
                    new List<string>(){
                        EndTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S05",
                    new List<string>(){
                        StartTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                        EndTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S06",
                    new List<string>(){
                        StartTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                        EndTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S07",
                    new List<string>(){
                        StartTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S08",
                    new List<string>(){
                        EndTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S09",
                    new List<string>(){
                        StartTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion),
                        EndTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S10",
                    new List<string>(){
                        StartimeIsAfterEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S11",
                    new List<string>(){
                        StartimeIsAfterEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                 {
                    "S12",
                    new List<string>(){
                        StartimeIsAfterEndTimeMessage(salesArea, breakExclusion),
                    }
                },
                {
                    "S13",
                    new List<string>(){
                        StartimeIsAfterEndTimeMessage(salesArea, breakExclusion),
                    }
                },
            };

            var breakExclusionsList = new List<BreakExclusionModel>() { breakExclusion };
            var salesAreas = new List<SalesArea>() { salesArea };
            List<string> errorMessages;
            Assert.IsFalse(BreakExclusionsValidations.DateTimeRangeIsValid(breakExclusionsList, salesAreas, out errorMessages));
            Assert.AreEqual(expectedMessages[salesAreaName].Count, errorMessages.Count);
            foreach (var errorMessage in errorMessages)
            {
                Assert.IsNotNull(expectedMessages[salesAreaName].Find(expectedMessage => string.Equals(expectedMessage, errorMessage, StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        private static List<BreakExclusionModel> GetBreakExclusionsList(string salesAreaName, DateTime startDate
            , DateTime endDate, string breakExclusionStartTime, string breakExclusionEndTime)
        {
            return new List<BreakExclusionModel>()
            {
               GetBreakExclusion(salesAreaName, startDate, endDate, breakExclusionStartTime, breakExclusionEndTime)
            };
        }

        private static BreakExclusionModel GetBreakExclusion(string salesAreaName, DateTime startDate
           , DateTime endDate, string breakExclusionStartTime, string breakExclusionEndTime)
        {
            TimeSpan? startTime = null;
            TimeSpan? endTime = null;
            TimeSpan result;
            if (TimeSpan.TryParse(breakExclusionStartTime, CultureInfo.InvariantCulture, out result))
            {
                startTime = result;
            }
            if (TimeSpan.TryParse(breakExclusionEndTime, CultureInfo.InvariantCulture, out result))
            {
                endTime = result;
            }

            return new BreakExclusionModel()
            {
                StartDate = startDate,
                EndDate = endDate,
                StartTime = startTime,
                EndTime = endTime,
                SalesArea = salesAreaName
            };
        }

        private static List<SalesArea> GetSalesAreas(string salesAreaName, string broadcastStartTime,
            string broadcastDuration)
        {
            return new List<SalesArea>() {
                GetSalesArea(salesAreaName, broadcastStartTime, broadcastDuration)
            };
        }

        private static SalesArea GetSalesArea(string salesAreaName, string broadcastStartTime,
            string broadcastDuration)
        {
            return
                new SalesArea()
                {
                    Name = salesAreaName,
                    StartOffset = Duration.FromTimeSpan(TimeSpan.Parse(broadcastStartTime, CultureInfo.InvariantCulture)),
                    DayDuration = Duration.FromTimeSpan(TimeSpan.Parse(broadcastDuration, CultureInfo.InvariantCulture)),
                };
        }

        private static string SalesAreaDescription(SalesArea salesArea)
        {
            return string.Format(
                CultureInfo.CurrentCulture
                , SalesAreaDescriptionFormat
                , salesArea.ShortName
                , salesArea.Name
                , salesArea.StartOffset.ToTimeSpan()
                , salesArea.DayDuration.TotalHours);
        }

        private static string EndDateBeforeStartDateMessage(SalesArea salesArea, BreakExclusionModel breakExclusion)
        {
            return string.Format(
                 CultureInfo.CurrentCulture
                 , EndDateBeforeStartDateMessageFormat
                 , SalesAreaDescription(salesArea)
                 , breakExclusion.EndDate.ToShortDateString()
                 , breakExclusion.StartDate.ToShortDateString());
        }

        private static string StartimeIsAfterEndTimeMessage(SalesArea salesArea, BreakExclusionModel breakExclusion)
        {
            return string.Format(
                 CultureInfo.CurrentCulture
                 , StartimeIsAfterEndTimeMessageFormat
                 , SalesAreaDescription(salesArea)
                 , breakExclusion.EndTime
                 , breakExclusion.StartTime);
        }

        private static string EndTimeIsAfterBroadcastEndTimeMessage(SalesArea salesArea, BreakExclusionModel breakExclusion)
        {
            return string.Format(
                 CultureInfo.CurrentCulture
                 , EndTimeIsAfterBroadcastEndTimeMessageFormat
                 , SalesAreaDescription(salesArea)
                 , breakExclusion.EndTime
                 , salesArea.StartOffset.ToTimeSpan().Add(salesArea.DayDuration.ToTimeSpan()).Subtract(new TimeSpan(0, 0, 1)));
        }

        private static string StartTimeIsAfterBroadcastEndTimeMessage(SalesArea salesArea, BreakExclusionModel breakExclusion)
        {
            return string.Format(
                 CultureInfo.CurrentCulture
                 , StartTimeIsAfterBroadcastEndTimeMessageFormat
                 , SalesAreaDescription(salesArea)
                 , breakExclusion.StartTime
                 , salesArea.StartOffset.ToTimeSpan().Add(salesArea.DayDuration.ToTimeSpan()).Subtract(new TimeSpan(0, 0, 1)));
        }

        private static string StartTimeIsBeforeBroadcastStartTimeMessage(SalesArea salesArea, BreakExclusionModel breakExclusion)
        {
            return string.Format(
                 CultureInfo.CurrentCulture
                 , StartTimeIsBeforeBroadcastStartTimeMessageFormat
                 , SalesAreaDescription(salesArea)
                 , breakExclusion.StartTime
                 , salesArea.StartOffset.ToTimeSpan());
        }

        private static string EndTimeIsBeforeBroadcastStartTimeMessage(SalesArea salesArea, BreakExclusionModel breakExclusion)
        {
            return string.Format(
                 CultureInfo.CurrentCulture
                 , EndTimeIsBeforeBroadcastStartTimeMessageFormat
                 , SalesAreaDescription(salesArea)
                 , breakExclusion.EndTime
                 , salesArea.StartOffset.ToTimeSpan());
        }
    }
}
