using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ProgrammeCategory
{
    public class BulkProgrammeCategoryDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkProgrammeCategoryDeleted>
    {
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;

        public BulkProgrammeCategoryDeletedEventHandler(IProgrammeCategoryHierarchyRepository programmeCategoryRepository)
        {
            _programmeCategoryRepository = programmeCategoryRepository;
        }

        public override void Handle(IBulkProgrammeCategoryDeleted command)
        {
            _programmeCategoryRepository.Truncate();
            _programmeCategoryRepository.SaveChanges();
        }
    }
}
