using System;

namespace ImagineCommunications.BusClient.Abstraction.Models
{
    public class DataSyncException<T> : Exception where T : Enum
    {
        public T ErrorCode { get; set; }

        public DataSyncException(T errorCode) : base(errorCode.ToString())
        {
            ErrorCode = errorCode;
        }

        public DataSyncException(T errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public DataSyncException(T errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
