using System;

namespace ImagineCommunications.Gameplan.Synchronization.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a concurrency violation is encountered while saving to the database.
    /// </summary>
    public class DbConcurrencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbConcurrencyException" /> class.
        /// </summary>
        public DbConcurrencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DbConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
