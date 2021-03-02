using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Holidays
{
    public class BulkHolidayCreatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkHolidayCreated>
    {
        private readonly ISalesAreaRepository _salesAreaRepository;

        public BulkHolidayCreatedEventHandler(ISalesAreaRepository salesAreaRepository)
        {
            _salesAreaRepository = salesAreaRepository;
        }

        public override void Handle(IBulkHolidayCreated command)
        {
            foreach (var holiday in command.Data)
            {
                List<SalesArea> salesAreas = GetSalesAreasFromDatabase(holiday.SalesAreaNames);

                salesAreas.ForEach(salesArea =>
                {
                    Add(salesArea, holiday.HolidayDateRanges, holiday.HolidayType);
                });

                _salesAreaRepository.Update(salesAreas);
            }

            _salesAreaRepository.SaveChanges();
        }

        private void Add(SalesArea salesArea, List<DateRange> newDateRanges, HolidayType holidayType)
        {
            var data = new List<ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges.DateRange>();
            switch (holidayType)
            {
                case HolidayType.PublicHoliday:
                    data = salesArea.PublicHolidays = salesArea.PublicHolidays ?? new List<GamePlan.Domain.Generic.Types.Ranges.DateRange>();
                    break;
                case HolidayType.SchoolHoliday:
                    data = salesArea.SchoolHolidays = salesArea.SchoolHolidays ?? new List<GamePlan.Domain.Generic.Types.Ranges.DateRange>();
                    break;
                default:
                    throw new Exception("Invalid holiday type");
            }

            if (data.Any())
            {
                foreach (var item in newDateRanges.Where(item => !data.Any(c => c.End == item.End && c.Start == item.Start)))
                {
                    data.Add(new GamePlan.Domain.Generic.Types.Ranges.DateRange()
                    {
                        End = item.End,
                        Start = item.Start
                    });
                }
            }
            else
            {
                data.AddRange(newDateRanges.Select(item => new GamePlan.Domain.Generic.Types.Ranges.DateRange() {End = item.End, Start = item.Start}));
            }
            
        }

        private List<SalesArea> GetSalesAreasFromDatabase(List<string> salesAreaNames)
        {
            var salesAreas = salesAreaNames == null || !salesAreaNames.Any()
                ? _salesAreaRepository.GetAll().ToList()
                : _salesAreaRepository.FindByNames(salesAreaNames);

            if (salesAreas == null || !salesAreas.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.SalesAreaNotFound, "SalesArea not found");
            }
            return salesAreas;
        }
    }
}
