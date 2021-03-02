using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ProgrammeClassification
{
    public class BulkProgrammeClassificationCreatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkProgrammeClassificationCreated>
    {
        private readonly IMapper _mapper;
        private readonly IProgrammeClassificationRepository _programmeClassificationRepository;

        public BulkProgrammeClassificationCreatedEventHandler(IMapper mapper, IProgrammeClassificationRepository programmeClassificationRepository)
        {
            _mapper = mapper;
            _programmeClassificationRepository = programmeClassificationRepository;
        }

        public override void Handle(IBulkProgrammeClassificationCreated command)
        {
            var pcs = _programmeClassificationRepository.GetAll().ToList();

            foreach (var pc in command.Data)
            {
                if (pcs.Any(x => x.Uid == pc.Uid))
                {
                    throw new DataSyncException(DataSyncErrorCode.UniqueConstraintViolation, "Uid must be unique");
                }
                if (pcs.Any(x => x.Code == pc.Code))
                {
                    throw new DataSyncException(DataSyncErrorCode.UniqueConstraintViolation, "Code must be unique");
                }
                if (pcs.Any(x => x.Description == pc.Description))
                {
                    throw new DataSyncException(DataSyncErrorCode.UniqueConstraintViolation, "Description must be unique");
                }
            }

            var programmeClassifications = _mapper.Map<List<GamePlan.Domain.Shared.ProgrammeClassifications.ProgrammeClassification>>(command.Data.ToList());

            _programmeClassificationRepository.Add(programmeClassifications);
            _programmeClassificationRepository.SaveChanges();
        }
    }
}
