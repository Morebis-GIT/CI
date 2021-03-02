using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.DayPartGroups
{
    public class BulkStandardDayPartGroupCreatedEventHandler : IEventHandler<IBulkStandardDayPartGroupCreated>
    {
        private readonly IStandardDayPartGroupRepository _standardDayPartGroupRepository;
        private readonly IStandardDayPartRepository _standardDayPartRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly IMapper _mapper;

        public BulkStandardDayPartGroupCreatedEventHandler(
            IStandardDayPartGroupRepository standardDayPartGroupRepository,
            IStandardDayPartRepository standardDayPartRepository,
            ISalesAreaRepository salesAreaRepository,
            IDemographicRepository demographicRepository,
            IMapper mapper)
        {
            _standardDayPartGroupRepository = standardDayPartGroupRepository;
            _standardDayPartRepository = standardDayPartRepository;
            _salesAreaRepository = salesAreaRepository;
            _demographicRepository = demographicRepository;
            _mapper = mapper;
        }

        public void Handle(IBulkStandardDayPartGroupCreated command)
        {
            var daypartGroups = _mapper.Map<List<StandardDayPartGroup>>(command.Data);

            _standardDayPartRepository.ValidateDayParts(command.Data.Where(c => c.Splits != null && c.Splits.Any()).SelectMany(c => c.Splits.Select(d => d.DayPartId)).ToList());
            _salesAreaRepository.ValidateSalesArea(daypartGroups.Select(c => c.SalesArea).ToList());
            _demographicRepository.ValidateDemographics(daypartGroups.Select(c => c.Demographic).ToList());

            _standardDayPartGroupRepository.AddRange(daypartGroups);
            _standardDayPartGroupRepository.SaveChanges();
        }
    }
}
