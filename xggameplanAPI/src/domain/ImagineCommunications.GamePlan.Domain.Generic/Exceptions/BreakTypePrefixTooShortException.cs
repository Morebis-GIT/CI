using System;
using System.Runtime.Serialization;

namespace ImagineCommunications.GamePlan.Domain.Generic.Exceptions
{
    [Serializable]
    public class BreakTypePrefixTooShortException
        : BreakTypeException
    {
        public BreakTypePrefixTooShortException()
            : this("The BreakType prefix value is too short. It must be at least two alphanumeric characters.")
        {
        }

        public BreakTypePrefixTooShortException(string message) : base(message)
        {
        }

        public BreakTypePrefixTooShortException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BreakTypePrefixTooShortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
