using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using xggameplan.Model;

namespace xggameplan.Validations.Passes
{
    public static class BreakExclusionsValidations
    {
        const string SalesAreaDescriptionFormat = "[Name: '{0} ({1})', StartTime: '{2}', Duration(hours): '{3}']";
        const string EndDateBeforeStartDateMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndDate = {1}' must be after  'StartDate = {2}'";
        const string StartimeIsAfterEndTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndTime = {1}' must be after 'StartTime = {2}'";
        const string EndTimeIsAfterBroadcastEndTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndTime = {1}' must be before 'Broadcast.EndTime = {2}'";
        const string StartTimeIsAfterBroadcastEndTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'StartTime = {1}' must be before 'Broadcast.EndTime = {2}'";
        const string StartTimeIsBeforeBroadcastStartTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'StartTime = {1}' must be equal or after 'Broadcast.StartTime = {2}'";
        const string EndTimeIsBeforeBroadcastStartTimeMessageFormat = "Break exclusion date/time validation error for SalesArea: {0}, breakExclusion 'EndTime = {1}' must be after 'Broadcast.StartTime = {2}'";

        public static bool DateTimeRangeIsValid(BreakExclusionModel breakExclusion, IEnumerable<SalesArea> salesAreas,out List<string> errorMessages)
        {
            errorMessages = new List<string>();
            if (breakExclusion == null)
            {
                return true;
            }
            if (salesAreas == null)
            {
                return true;
            }

            var salesArea = salesAreas.FirstOrDefault(sa => string.Equals(sa.Name, breakExclusion.SalesArea));

            if (salesArea == null)
            {
                return true;
            }
            if (!(breakExclusion.StartTime.HasValue && breakExclusion.EndTime.HasValue))
            {
                return true;
            }
            var zero = new TimeSpan(0, 0, 0);
            var nextDay = new TimeSpan(1, 0, 0, 0);
            var startOffset = salesArea.StartOffset.ToTimeSpan();
            var duration = salesArea.DayDuration.ToTimeSpan();
            var startTime = breakExclusion.StartTime.Value;
            var endTime = breakExclusion.EndTime.Value;
            //Check if startTime needs to be on the next day
            if (startTime >= zero && startTime < startOffset && startTime.Add(nextDay) < startOffset.Add(duration))
            {
                startTime = startTime.Add(nextDay);
            }
            //Check if endTime needs to be on the next day
            if (endTime >= zero && endTime < startTime && endTime.Add(nextDay) < startOffset.Add(duration))
            {
                endTime = endTime.Add(nextDay);
            }
            //shift back start and end time to zero baseline
            startTime = startTime.Subtract(startOffset);
            endTime = endTime.Subtract(startOffset);
            if (breakExclusion.EndDate < breakExclusion.StartDate)
            {
                errorMessages.Add(EndDateBeforeStartDateMessage(salesArea, breakExclusion));
            }
            //startTime Is Before Broadcast Time Range
            if (startTime < zero)
            {
                errorMessages.Add(StartTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion));
            }
            //startTime Is After Broadcast Time Range
            if (startTime >= duration)
            {
                errorMessages.Add(StartTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion));
            }
            //endTime Is Before Broadcast Time Range
            if (endTime <= zero)
            {
                errorMessages.Add(EndTimeIsBeforeBroadcastStartTimeMessage(salesArea, breakExclusion));
            }
            //endTime is not after startTime
            if (endTime <= startTime)
            {
                errorMessages.Add(StartimeIsAfterEndTimeMessage(salesArea, breakExclusion));
            }
            //endTime Is After Broadcast Time Range
            if (endTime >= duration)
            {
                errorMessages.Add(EndTimeIsAfterBroadcastEndTimeMessage(salesArea, breakExclusion));
            }
            return errorMessages.Count == 0;
        }

        public static bool DateTimeRangeIsValid(List<BreakExclusionModel> breakExclusionModels, IEnumerable<SalesArea> salesAreas, out List<string> errorMessages)
        {
            errorMessages = new List<string>();
            if (breakExclusionModels is null)
            {
                return true;
            }

            foreach (var breakExclusion in breakExclusionModels)
            {
                List<string> errors=new List<string>();
                if (!DateTimeRangeIsValid(breakExclusion, salesAreas, out errors))
                {
                    errorMessages.AddRange(errors);
                }
            }
            return errorMessages.Count == 0;
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
                 , salesArea.StartOffset.ToTimeSpan().Add(salesArea.DayDuration.ToTimeSpan()).Subtract(new TimeSpan(0,0,1)));
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
