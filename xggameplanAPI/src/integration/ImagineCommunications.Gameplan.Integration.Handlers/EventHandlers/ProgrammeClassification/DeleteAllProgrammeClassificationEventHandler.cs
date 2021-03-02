using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ProgrammeClassification
{
    public class DeleteAllProgrammeClassificationEventHandler : EventHandler<IDeleteAllProgrammeClassification>
    {
        private readonly IProgrammeClassificationRepository _programmeClassificationRepository;

        public DeleteAllProgrammeClassificationEventHandler(IProgrammeClassificationRepository programmeClassificationRepository)
            => _programmeClassificationRepository = programmeClassificationRepository;

        public override void Handle(IDeleteAllProgrammeClassification command)
        {
            _programmeClassificationRepository.Truncate();
            _programmeClassificationRepository.SaveChanges();
        }
    }
}
