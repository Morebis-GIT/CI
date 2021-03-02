using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ClashExceptions
{
    public class BulkClashExceptionDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkClashExceptionDeleted>
    {
        private readonly IClashExceptionRepository _clashExceptionRepository;

        public BulkClashExceptionDeletedEventHandler(IClashExceptionRepository clashExceptionRepository) =>
            _clashExceptionRepository = clashExceptionRepository;

        public override void Handle(IBulkClashExceptionDeleted command)
        {
            _clashExceptionRepository.DeleteRangeByExternalRefs(command.Data.Select(c => c.ExternalRef));
            _clashExceptionRepository.SaveChanges();
        }
    }
}
