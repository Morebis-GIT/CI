using System;

namespace xggameplan.core.Services.RunCleaning
{
    /// <summary>
    /// Represents the exception thrown while run deletion.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RunCleaningException : Exception
    {
        public RunCleaningException()
        {
        }

        public RunCleaningException(string message) : base(message)
        {
        }

        public RunCleaningException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
