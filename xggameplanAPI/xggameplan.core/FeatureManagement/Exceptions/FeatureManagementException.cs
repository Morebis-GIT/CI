using System;
using System.Runtime.Serialization;

namespace xggameplan.core.FeatureManagement.Exceptions
{
    [Serializable]
    public class FeatureManagementException : Exception
    {
        public FeatureManagementException()
        {
        }

        public FeatureManagementException(string message) : base(message)
        {
        }

        public FeatureManagementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FeatureManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
