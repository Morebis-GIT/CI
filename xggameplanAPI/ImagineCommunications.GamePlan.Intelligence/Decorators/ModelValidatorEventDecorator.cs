using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Decorators
{
    public class ModelValidatorEventDecorator<T> : IEventHandler<T> where T : IEvent
    {
        private readonly IEventHandler<T> _eventHandler;
        private readonly IContractValidatorService _contractValidatorService;
        public ModelValidatorEventDecorator(IEventHandler<T> eventHandler, IContractValidatorService contractValidatorService)
        {
            _eventHandler = eventHandler;
            _contractValidatorService = contractValidatorService;
        }

        public void Handle(T command)
        {
            _contractValidatorService.Validate<T>(command);
            _eventHandler.Handle(command);
        }
    }
}
