using System;
using System.Runtime.Serialization;

namespace ImagineCommunications.GamePlan.Domain.Generic.Exceptions
{
    [Serializable]
    public class BreakTypeNullException
        : BreakTypeException
    {
        public BreakTypeNullException()
            : this("The BreakType value cannot be null.")
        {
        }

        public BreakTypeNullException(string message)
            : base(message)
        {
        }

        public BreakTypeNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BreakTypeNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
