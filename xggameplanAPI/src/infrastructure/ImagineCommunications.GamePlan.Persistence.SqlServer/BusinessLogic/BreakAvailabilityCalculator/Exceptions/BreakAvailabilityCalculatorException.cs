using System;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using xggameplan.common.Helpers;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Exceptions
{
    public class BreakAvailabilityCalculatorException : Exception
    {
        public BreakAvailabilityCalculatorException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        public BreakAvailabilityCalculatorException(DateTimeRange period, string salesAreaName,
            Exception innerException)
            : base(
                $"Break availability calculation for '{salesAreaName}' sales area on {LogAsString.Log(period.Start.Date)} has thrown an exception.",
                innerException)
        {
        }
    }
}
