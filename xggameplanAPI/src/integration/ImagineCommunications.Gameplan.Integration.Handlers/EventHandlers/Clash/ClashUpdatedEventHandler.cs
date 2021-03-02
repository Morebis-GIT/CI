using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Clash
{
    public class ClashUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IClashUpdated>
    {
        private readonly IMapper _mapper;
        private readonly IClashRepository _clashRepository;

        public ClashUpdatedEventHandler(IMapper mapper, IClashRepository clashRepository)
        {
            _mapper = mapper;
            _clashRepository = clashRepository;
        }

        public override void Handle(IClashUpdated command)
        {
            var clash = _clashRepository.CheckClashByExternalRef(command.Externalref);

            clash.ParentExternalidentifier = command.ParentExternalidentifier;
            clash.Description = command.Description;
            clash.DefaultPeakExposureCount = command.ExposureCount;
            clash.DefaultOffPeakExposureCount = command.ExposureCount;
            clash.Differences = _mapper.Map<List<ClashDifference>>(command.Differences);

            foreach (var difference in clash.Differences)
            {
                difference.TimeAndDow = new GamePlan.Domain.Generic.Types.TimeAndDowAPI
                {
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(5, 59, 59)
                };
                difference.TimeAndDow.SetDaysOfWeek("1111111");
            }

            if (!string.IsNullOrWhiteSpace(clash.ParentExternalidentifier))
            {
                _clashRepository.CheckClashParents(new[] { clash });
            }

            _clashRepository.Add(clash);
            _clashRepository.SaveChanges();
        }
    }
}
