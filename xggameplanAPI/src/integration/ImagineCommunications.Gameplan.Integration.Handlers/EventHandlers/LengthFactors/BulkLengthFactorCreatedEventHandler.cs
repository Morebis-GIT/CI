using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.LengthFactor
{
    public class BulkLengthFactorCreatedEventHandler : EventHandler<IBulkLengthFactorCreated>
    {
        private readonly IMapper _mapper;
        private readonly ILengthFactorRepository _repository;
        private readonly ISalesAreaRepository _salesAreaRepository;

        public BulkLengthFactorCreatedEventHandler(ILengthFactorRepository repository, ISalesAreaRepository salesAreaRepository,IMapper mapper)
        {
            _repository = repository;
            _salesAreaRepository = salesAreaRepository;
            _mapper = mapper;
        }

        public override void Handle(IBulkLengthFactorCreated command)
        {
            var salesAreaNames = command.Data.Select(x => x.SalesArea).Distinct().ToList();
            _salesAreaRepository.ValidateSalesArea(salesAreaNames);

            var lengthFactors = _mapper.Map<List<GamePlan.Domain.LengthFactors.LengthFactor>>(command.Data);
            _repository.AddRange(lengthFactors);
            _repository.SaveChanges();
        }
    }
}
