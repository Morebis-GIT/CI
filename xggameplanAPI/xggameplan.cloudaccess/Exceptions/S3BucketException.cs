using System;

namespace xggameplan.cloudaccess.Exceptions
{
    public class S3BucketException : Exception
    {
        public S3BucketException()
        {
        }

        public S3BucketException(string message) : base(message)
        {
        }

        public S3BucketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
