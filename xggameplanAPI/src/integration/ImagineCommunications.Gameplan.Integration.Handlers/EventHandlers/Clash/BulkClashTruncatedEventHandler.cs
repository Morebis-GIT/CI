using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Clash
{
    public class BulkClashTruncatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IClashTruncated>
    {
        private readonly IClashRepository _clashRepository;
        private readonly ILoggerService _logger;

        public BulkClashTruncatedEventHandler(IClashRepository clashRepository, ILoggerService logger)
        {
            _clashRepository = clashRepository;
            _logger = logger;
        }

        public override void Handle(IClashTruncated command)
        {
            _clashRepository.Truncate();
        }
    }
}
