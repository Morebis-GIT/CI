using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Abstraction.Classes
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public abstract void Handle(TCommand command);
    }
}
