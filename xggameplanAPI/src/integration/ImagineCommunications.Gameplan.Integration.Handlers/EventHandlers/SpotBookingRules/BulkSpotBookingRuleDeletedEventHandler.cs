using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SpotBookingRules
{
    public class BulkSpotBookingRuleDeletedEventHandler : IEventHandler<IBulkSpotBookingRuleDeleted>
    {
        private readonly ISpotBookingRuleRepository _spotBookingRuleRepository;

        public BulkSpotBookingRuleDeletedEventHandler(ISpotBookingRuleRepository spotBookingRuleRepository)
        {
            _spotBookingRuleRepository = spotBookingRuleRepository;
        }

        public void Handle(IBulkSpotBookingRuleDeleted command)
        {
            _spotBookingRuleRepository.Truncate();
            _spotBookingRuleRepository.SaveChanges();
        }
    }
}
