using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Restriction
{
    public class BulkRestrictionDeletedEventHandler : EventHandler<IBulkRestrictionDeleted>
    {
        private readonly IRestrictionRepository _restrictionRepository;

        public BulkRestrictionDeletedEventHandler(IRestrictionRepository restrictionRepository) =>
            _restrictionRepository = restrictionRepository;

        public override void Handle(IBulkRestrictionDeleted command)
        {
            _restrictionRepository.DeleteRangeByExternalRefs(command.Data.Select(r => r.ExternalReference));
            _restrictionRepository.SaveChanges();
        }
    }
}
