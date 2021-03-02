using System;
using System.Runtime.Serialization;

namespace xggameplan.core.FeatureManagement.Exceptions
{
    [Serializable]
    public class FeatureInitializeException : FeatureManagementException
    {
        public FeatureInitializeException()
        {
        }

        public FeatureInitializeException(string message) : base(message)
        {
        }

        public FeatureInitializeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FeatureInitializeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
