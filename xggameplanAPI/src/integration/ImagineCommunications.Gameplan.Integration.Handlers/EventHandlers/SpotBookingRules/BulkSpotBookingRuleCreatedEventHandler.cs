using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SpotBookingRules
{
    public class BulkSpotBookingRuleCreatedEventHandler : IEventHandler<IBulkSpotBookingRuleCreated>
    {
        private readonly ISpotBookingRuleRepository _spotBookingRuleRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly IMapper _mapper;

        public BulkSpotBookingRuleCreatedEventHandler(
            ISpotBookingRuleRepository spotBookingRuleRepository, 
            ISalesAreaRepository salesAreaRepository, 
            IMetadataRepository metadataRepository, 
            IMapper mapper)
        {
            _spotBookingRuleRepository = spotBookingRuleRepository;
            _salesAreaRepository = salesAreaRepository;
            _metadataRepository = metadataRepository;
            _mapper = mapper;
        }

        public void Handle(IBulkSpotBookingRuleCreated command)
        {
            var spotBookingRules = _mapper.Map<List<SpotBookingRule>>(command.Data);

            _salesAreaRepository.ValidateSalesArea(command.Data.Where(c => c.SalesAreas != null && c.SalesAreas.Any()).SelectMany(c => c.SalesAreas).ToList());
            _metadataRepository.ValidateBreakType(command.Data.Select(c=>c.BreakType).ToList());

            _spotBookingRuleRepository.AddRange(spotBookingRules);
            _spotBookingRuleRepository.SaveChanges();
        }
    }
}
