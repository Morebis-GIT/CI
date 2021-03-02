using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IServiceBus : IDisposable
    {
        Task<Guid> PublishAsync<T>(T eventMessage, Guid? groupTransactionId = null, Dictionary<string, object> headers = null) where T : class, IEvent;

        Task SendAsync<T>(T commandMessage) where T : class, ICommand;

        void Start();

        void Stop();

        Task StartAsync();

        Task StopAsync();
    }
}
