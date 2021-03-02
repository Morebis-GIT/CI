using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;
using xggameplan.core.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Spot
{
    public class BulkSpotDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkSpotDeleted>
    {
        private readonly ISpotCleaner _spotCleaner;
        private readonly ILoggerService _loggerService;

        public BulkSpotDeletedEventHandler(ISpotCleaner spotCleaner, ILoggerService loggerService)
        {
            _spotCleaner = spotCleaner;
            _loggerService = loggerService;
        }

        public override void Handle(IBulkSpotDeleted command)
        {
            var externalRefs = command.Data.Select(s => s.ExternalSpotRef).ToList();

            Task.Run(async() => {
                await _spotCleaner.ExecuteAsync(externalRefs, (string message) => _loggerService.Info(message)).ConfigureAwait(false);
            }).GetAwaiter().GetResult();
        }
    }
}
