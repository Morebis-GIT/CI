using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using System.Linq;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Holidays
{
    public class BulkHolidayDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkHolidayDeleted>
    {
        private readonly ISalesAreaRepository _salesAreaRepository;

        public BulkHolidayDeletedEventHandler(ISalesAreaRepository salesAreaRepository)
        {
            _salesAreaRepository = salesAreaRepository;
        }

        public override void Handle(IBulkHolidayDeleted command)
        {
            var salesAreas = _salesAreaRepository.GetAll().ToList();

            foreach (var commandItem in command.Data)
            {
                foreach (var salesArea in salesAreas)
                {
                    salesArea.PublicHolidays?.RemoveAll(c => c.Start >= commandItem.StartDate && c.End <= commandItem.EndDate);
                    salesArea.SchoolHolidays?.RemoveAll(c => c.Start >= commandItem.StartDate && c.End <= commandItem.EndDate);
                }
            }

            _salesAreaRepository.Update(salesAreas);
            _salesAreaRepository.SaveChanges();
        }
    }
}
