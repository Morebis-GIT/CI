using System;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Exceptions
{
    public class DataPurgingException : Exception
    {
        public DataPurgingException()
        {
        }

        public DataPurgingException(string message) : base(message)
        {
        }

        public DataPurgingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
