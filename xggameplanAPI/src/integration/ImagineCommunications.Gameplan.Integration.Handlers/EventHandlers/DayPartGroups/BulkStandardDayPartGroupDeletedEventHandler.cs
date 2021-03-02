using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.DayPartGroups
{
    public class BulkStandardDayPartGroupDeletedEventHandler : IEventHandler<IBulkStandardDayPartGroupDeleted>
    {
        private readonly IStandardDayPartGroupRepository _standardDayPartGroupRepository;

        public BulkStandardDayPartGroupDeletedEventHandler(IStandardDayPartGroupRepository standardDayPartGroupRepository)
        {
            _standardDayPartGroupRepository = standardDayPartGroupRepository;
        }

        public void Handle(IBulkStandardDayPartGroupDeleted command)
        {
            _standardDayPartGroupRepository.Truncate();
            _standardDayPartGroupRepository.SaveChanges();
        }
    }
}
