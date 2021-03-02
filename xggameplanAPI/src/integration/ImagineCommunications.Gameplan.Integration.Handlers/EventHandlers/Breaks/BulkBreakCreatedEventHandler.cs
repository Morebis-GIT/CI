using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Breaks
{
    public class BulkBreakCreatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkBreakCreated>
    {
        private readonly IBreakRepository _breakRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly IMapper _mapper;

        public BulkBreakCreatedEventHandler(
            IBreakRepository breakRepository,
            IScheduleRepository scheduleRepository,
            ISalesAreaRepository salesAreaRepository,
            IMetadataRepository metadataRepository,
            IMapper mapper)
        {
            _breakRepository = breakRepository;
            _scheduleRepository = scheduleRepository;
            _salesAreaRepository = salesAreaRepository;
            _metadataRepository = metadataRepository;
            _mapper = mapper;
        }

        public override void Handle(IBulkBreakCreated command)
        {
            _metadataRepository.ValidateBreakType(command.Data.Select(b => b.BreakType).ToList());
            _salesAreaRepository.ValidateSalesArea(command.Data.Select(s => s.SalesArea).ToList());

            var salesAreaDictionary = _salesAreaRepository.GetAll()
                .ToDictionary(x => x.Name, x => x.CustomId);

            var breaks = _mapper.Map<List<Break>>(command.Data);

            // Populate break data counting custom ID from zero for each day
            var breakGroups = breaks.GroupBy(s => new { s.ScheduledDate.Date, s.SalesArea });
            foreach (var breakGroup in breakGroups)
            {
                PopulateBreakProperties(breakGroup.ToList(), salesAreaDictionary);
            }

            _breakRepository.Add(breaks);

            // Note: this implies that any batch of breaks recieved should be treated as the only
            // schedule breaks for a schedule day ie. overwrite list not append to list
            foreach (var breakGroup in breakGroups)
            {
                var schedule = _scheduleRepository.GetSchedule(breakGroup.Key.SalesArea, breakGroup.Key.Date) ??
                    new Schedule { SalesArea = breakGroup.Key.SalesArea, Date = breakGroup.Key.Date };

                schedule.Breaks = breakGroup.ToList();
                _scheduleRepository.Add(schedule);
            }

            _scheduleRepository.SaveChanges();
        }

        private static void PopulateBreakProperties(List<Break> breaksList, Dictionary<string, int> salesAreaDict)
        {
            var breakId = 0;
            breaksList.ForEach(b =>
            {
                b.Id = Guid.NewGuid();
                b.CustomId = ++breakId;
                b.Avail = b.Duration;
                b.OptimizerAvail = b.Duration;
                b.ExternalBreakRef = b.ExternalBreakRef.GenerateBreakExternalRef(
                    salesAreaDict[b.SalesArea], b.ScheduledDate);
            });
        }
    }
}
