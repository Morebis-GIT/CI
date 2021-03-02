using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Demographics
{
    public class BulkDemographicDeletedEventHandler : EventHandler<IBulkDemographicDeleted>
    {
        private readonly IDemographicRepository _demographicRepository;

        public BulkDemographicDeletedEventHandler(IDemographicRepository demographicRepository)
        {
            _demographicRepository = demographicRepository;
        }

        public override void Handle(IBulkDemographicDeleted command)
        {
            _demographicRepository.DeleteRangeByExternalRefs(command.Data.Select(c => c.ExternalRef));
            _demographicRepository.SaveChanges();
        }
    }
}
