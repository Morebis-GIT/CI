using System;

namespace xgCore.xgGamePlan.AutomationTests.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetDateInRange(this (DateTime from, DateTime to) dateRange)
        {
            var days = (dateRange.to - dateRange.from).TotalDays;
            return dateRange.from.AddDays(days / 2);
        }
    }
}
