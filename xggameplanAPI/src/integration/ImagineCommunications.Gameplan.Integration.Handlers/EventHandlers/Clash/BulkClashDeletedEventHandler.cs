using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Clash
{
    public class BulkClashDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkClashDeleted>
    {
        private readonly IClashRepository _clashRepository;
        private readonly ILoggerService _logger;

        public BulkClashDeletedEventHandler(IClashRepository clashRepository, ILoggerService logger)
        {
            _clashRepository = clashRepository;
            _logger = logger;
        }

        public override void Handle(IBulkClashDeleted command)
        {
            var clashes = _clashRepository.FindByExternal(command.Data.Select(c => c.Externalref).ToList()).ToList();
            if (clashes.Any())
            {
                _clashRepository.DeleteRange(clashes.Select(c => c.Uid));
                _clashRepository.SaveChanges();
            }
            else
            {
                _logger.Warn("Clashes can't be found with specified external references.");
            }
        }
    }
}
