using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Exceptions
{
    public class RecalculateBreakAvailabilityServiceException : Exception
    {
        public RecalculateBreakAvailabilityServiceException(string message)
            : base(message)
        {
        }

        public RecalculateBreakAvailabilityServiceException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
