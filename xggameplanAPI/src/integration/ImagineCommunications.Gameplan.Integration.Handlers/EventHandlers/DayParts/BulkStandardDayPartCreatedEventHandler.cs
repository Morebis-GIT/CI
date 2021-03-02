using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.DayParts
{
    public class BulkStandardDayPartCreatedEventHandler : IEventHandler<IBulkStandardDayPartCreated>
    {
        private readonly IStandardDayPartRepository _standardDayPartRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IMapper _mapper;

        public BulkStandardDayPartCreatedEventHandler(IStandardDayPartRepository standardDayPartRepository, ISalesAreaRepository salesAreaRepository, IMapper mapper)
        {
            _standardDayPartRepository = standardDayPartRepository;
            _salesAreaRepository = salesAreaRepository;
            _mapper = mapper;
        }
        public void Handle(IBulkStandardDayPartCreated command)
        {
            var dayParts = _mapper.Map<List<StandardDayPart>>(command.Data);

            _salesAreaRepository.ValidateSalesArea(dayParts.Select(c => c.SalesArea).ToList());

            _standardDayPartRepository.AddRange(dayParts);
            _standardDayPartRepository.SaveChanges();
        }
    }
}
