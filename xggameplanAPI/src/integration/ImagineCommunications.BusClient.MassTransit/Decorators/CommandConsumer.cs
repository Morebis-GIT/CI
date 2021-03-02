using System.Threading.Tasks;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using MassTransit;

namespace ImagineCommunications.BusClient.MassTransit.Decorators
{
    public class CommandConsumer<T> : ICommandHandler<T>, IConsumer<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _commandHandler;

        public CommandConsumer() { }

        public CommandConsumer(ICommandHandler<T> commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public Task Consume(ConsumeContext<T> context)
        {
            Handle(context.Message);
            return Task.CompletedTask;
        }

        public void Handle(T command)
        {
            _commandHandler.Handle(command);
        }
    }
}
