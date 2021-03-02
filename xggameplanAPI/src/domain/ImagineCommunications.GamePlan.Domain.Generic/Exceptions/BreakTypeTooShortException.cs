using System;
using System.Runtime.Serialization;

namespace ImagineCommunications.GamePlan.Domain.Generic.Exceptions
{
    [Serializable]
    public class BreakTypeTooShortException
        : BreakTypeException
    {
        public BreakTypeTooShortException()
            : this("The BreakType value is too short. It must be at least two alphanumeric characters.")
        {
        }

        public BreakTypeTooShortException(string message) : base(message)
        {
        }

        public BreakTypeTooShortException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BreakTypeTooShortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
