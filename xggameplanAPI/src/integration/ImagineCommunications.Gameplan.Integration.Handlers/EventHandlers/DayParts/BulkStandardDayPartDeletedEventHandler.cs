using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.DayParts
{
    public class BulkStandardDayPartDeletedEventHandler : IEventHandler<IBulkStandardDayPartDeleted>
    {
        private readonly IStandardDayPartRepository _standardDayPartRepository;

        public BulkStandardDayPartDeletedEventHandler(IStandardDayPartRepository standardDayPartRepository)
        {
            _standardDayPartRepository = standardDayPartRepository;
        }

        public void Handle(IBulkStandardDayPartDeleted command)
        {
            _standardDayPartRepository.Truncate();
            _standardDayPartRepository.SaveChanges();
        }
    }
}
