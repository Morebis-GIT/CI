using System;
using ImagineCommunications.BusClient.Abstraction.Models;

namespace ImagineCommunications.Gameplan.Integration.Handlers.Common
{
    public class DataSyncException : DataSyncException<DataSyncErrorCode>
    {
        public DataSyncException(DataSyncErrorCode errorCode) : base(errorCode) { }
        
        public DataSyncException(DataSyncErrorCode errorCode, string message) : base(errorCode, message) { }

        public DataSyncException(DataSyncErrorCode errorCode, string message, Exception innerException) : base(errorCode, message, innerException) { }
    }
}
