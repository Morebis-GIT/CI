using System.Threading.Tasks;
using MassTransit;

namespace ImagineCommunications.BusClient.MassTransit.Decorators
{
    public abstract class ErrorConsumer<T> : IConsumer<Fault<T>>
    {
        public virtual Task Consume(ConsumeContext<Fault<T>> context) => Task.CompletedTask;
    }
}
