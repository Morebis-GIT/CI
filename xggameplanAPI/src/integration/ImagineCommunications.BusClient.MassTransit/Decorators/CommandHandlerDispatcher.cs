using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Implementation.Decorators
{
    public class CommandHandlerDispatcher<TCommand> : IHandlerDispatcher
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly IContractValidatorService _contractValidatorService;
        public CommandHandlerDispatcher(ICommandHandler<TCommand> commandHandler, IContractValidatorService contractValidatorService)
        {
            _commandHandler = commandHandler;
            _contractValidatorService = contractValidatorService;
        }

        public void Dispatch(MessageInfo info, object message)
        {
            _contractValidatorService.Validate<TCommand>((TCommand)message);
            _commandHandler.Handle((TCommand)message);
        }
    }
}
