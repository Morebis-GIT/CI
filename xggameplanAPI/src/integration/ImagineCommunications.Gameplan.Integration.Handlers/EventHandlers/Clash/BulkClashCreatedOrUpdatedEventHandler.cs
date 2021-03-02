using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Clash
{
    public class BulkClashCreatedOrUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkClashCreatedOrUpdated>
    {
        private readonly IMapper _mapper;
        private readonly IClashRepository _clashRepository;

        public BulkClashCreatedOrUpdatedEventHandler(IMapper mapper, IClashRepository clashRepository)
        {
            _mapper = mapper;
            _clashRepository = clashRepository;
        }

        public override void Handle(IBulkClashCreatedOrUpdated command)
        {
            var clashModels = _mapper.Map<List<GamePlan.Domain.BusinessRules.Clashes.Objects.Clash>>(command.Data);

            _clashRepository.CheckClashParents(clashModels);

            var externalRefs = command.Data.Select(x => x.Externalref).ToList();
            var dbClashes = _clashRepository.FindByExternal(externalRefs);
            var newEntities = new List<GamePlan.Domain.BusinessRules.Clashes.Objects.Clash>();

            foreach (var item in clashModels)
            {
                var existingClash = dbClashes.SingleOrDefault(x => x.Externalref.Equals(item.Externalref, StringComparison.OrdinalIgnoreCase));

                foreach (var difference in item.Differences)
                {
                    difference.TimeAndDow = new GamePlan.Domain.Generic.Types.TimeAndDowAPI
                    {
                        StartTime = new TimeSpan(6, 0, 0),
                        EndTime = new TimeSpan(5, 59, 59)
                    };
                    difference.TimeAndDow.SetDaysOfWeek("1111111");
                }

                if (existingClash == null)
                {
                    newEntities.Add(item);
                }
                else
                {
                    existingClash.ParentExternalidentifier = item.ParentExternalidentifier;
                    existingClash.Description = item.Description;
                    existingClash.DefaultOffPeakExposureCount = item.ExposureCount;
                    existingClash.DefaultPeakExposureCount = item.ExposureCount;
                    existingClash.Differences = item.Differences;
                }
            }

            _clashRepository.Add(newEntities);
            _clashRepository.UpdateRange(dbClashes);
            _clashRepository.SaveChanges();
        }
    }
}
