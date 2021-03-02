using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BreakTypes
{
    public class BulkBreakTypeCreatedEventHandler : IEventHandler<IBulkBreakTypeCreated>
    {
        private readonly IMapper _mapper;
        private readonly IMetadataRepository _metadataRepository;

        public BulkBreakTypeCreatedEventHandler(IMapper mapper, IMetadataRepository metadataRepository)
        {
            _mapper = mapper;
            _metadataRepository = metadataRepository;
        }

        public void Handle(IBulkBreakTypeCreated command)
        {
            var metadataModel = _metadataRepository.GetAll();
            var breakTypeMetadataKey = MetaDataKeys.BreakTypes;

            if (!metadataModel.ContainsKey(breakTypeMetadataKey))
            {
                metadataModel.Add(breakTypeMetadataKey, new List<Data>());
            }

            var breakTypes = metadataModel[breakTypeMetadataKey] ?? new List<Data>();
            var breakTypesToAdd = command.Data.Where(x => !breakTypes.Any(bt => bt.Value.ToString()
                .Equals(x.Name, StringComparison.OrdinalIgnoreCase)));

            var newBreakTypes = _mapper.Map<List<Data>>(breakTypesToAdd);
            AppendId(breakTypes, newBreakTypes);

            breakTypes.AddRange(newBreakTypes);
            metadataModel[breakTypeMetadataKey] = breakTypes;

            _metadataRepository.Add(metadataModel);
            _metadataRepository.SaveChanges();
        }

        private void AppendId(List<Data> existingBreakTypes, List<Data> newBreakTypes)
        {
            var maxValueId = existingBreakTypes.Any() ? existingBreakTypes.Max(x => x.Id) : 0;
            foreach (var newBreakType in newBreakTypes)
            {
                newBreakType.Id = ++maxValueId;
            }
        }
    }
}
