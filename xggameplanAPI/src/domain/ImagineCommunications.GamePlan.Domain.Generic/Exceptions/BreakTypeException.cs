using System;
using System.Runtime.Serialization;

namespace ImagineCommunications.GamePlan.Domain.Generic.Exceptions
{
    [Serializable]
    public class BreakTypeException
        : Exception
    {
        public BreakTypeException()
        {
        }

        public BreakTypeException(string message)
            : base(message)
        {
        }

        public BreakTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BreakTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
