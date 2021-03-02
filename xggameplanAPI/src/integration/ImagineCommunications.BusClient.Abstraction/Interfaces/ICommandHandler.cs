namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
