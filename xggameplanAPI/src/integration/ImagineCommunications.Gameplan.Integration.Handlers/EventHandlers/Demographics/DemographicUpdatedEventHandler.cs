using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Demographics
{
    public class DemographicUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IDemographicUpdated>
    {
        private readonly IDemographicRepository _demographicsRepository;

        public DemographicUpdatedEventHandler(IDemographicRepository demographicsRepository)
        {
            _demographicsRepository = demographicsRepository;
        }

        public override void Handle(IDemographicUpdated command)
        {
            var existingDemographic = _demographicsRepository.CheckDemographicByExternalRef(command.ExternalRef);

            existingDemographic.Update(command.Name, command.ShortName, command.DisplayOrder, command.Gameplan);
            _demographicsRepository.Update(existingDemographic);
            _demographicsRepository.SaveChanges();
        }
    }
}
