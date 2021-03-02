using System;

namespace xggameplan.core.Exceptions
{
    public class LandmarkNotAvailableException : Exception
    {
        public const string DefaultMessage = "GamePlan is unable to connect with Landmark Sales, please contact IT for assistance";

        public LandmarkNotAvailableException() : this(DefaultMessage)
        {
        }

        public LandmarkNotAvailableException(Exception innerException) : base(DefaultMessage, innerException)
        {
        }

        public LandmarkNotAvailableException(string message) : base(message)
        {
        }

        public LandmarkNotAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
