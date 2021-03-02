using System;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public static class BreakAvailabilityCalculationExtensions
    {
        public static bool DateTimeIsInProgramme(this IProgrammeForBreakAvailCalculation programme, DateTime dateTime)
        {
            return programme != null && programme.StartDateTime <= dateTime &&
                   dateTime < programme.StartDateTime.Add(programme.Duration.ToTimeSpan());
        }
    }
}
