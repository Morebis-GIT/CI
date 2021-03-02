using System;

namespace xggameplan.specification.tests.Exceptions
{
    public class RepositoryAdapterException : Exception
    {
        public RepositoryAdapterException() : base()
        {
        }

        public RepositoryAdapterException(string message) : base(message)
        {
        }

        public RepositoryAdapterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
