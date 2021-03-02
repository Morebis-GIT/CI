using System;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IGroupTransactionExecutionService
    {
        EventHandler<Exception> OnError { get; set; }
        bool Execute();
    }
}
