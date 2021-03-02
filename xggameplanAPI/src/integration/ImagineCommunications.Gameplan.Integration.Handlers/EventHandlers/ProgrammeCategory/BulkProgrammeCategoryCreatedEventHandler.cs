using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ProgrammeCategory
{
    public class BulkProgrammeCategoryCreatedEventHandler : IEventHandler<IBulkProgrammeCategoryCreated>
    {
        private readonly IMapper _mapper;
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;

        public BulkProgrammeCategoryCreatedEventHandler(IMapper mapper, IProgrammeCategoryHierarchyRepository programmeCategoryRepository)
        {
            _mapper = mapper;
            _programmeCategoryRepository = programmeCategoryRepository;
        }

        public void Handle(IBulkProgrammeCategoryCreated command)
        {
            var existedItems = _programmeCategoryRepository.Search(command.Data.Select(p => p.Name));
            if (existedItems.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.UniqueConstraintViolation, "Name must be unique");
            }

            if (command.Data.Any(programmeCategory => !string.IsNullOrEmpty(programmeCategory.ExternalRef) &&
                !string.IsNullOrEmpty(programmeCategory.ParentExternalRef) &&
                programmeCategory.ExternalRef == programmeCategory.ParentExternalRef))
            {
                throw new DataSyncException(DataSyncErrorCode.DuplicateParentAndExternalReference,
                    "Parent and external references must not be the same");
            }
            var programmeCategories = _mapper.Map<List<ProgrammeCategoryHierarchy>>(command.Data.ToList());

            _programmeCategoryRepository.AddRange(programmeCategories);
            _programmeCategoryRepository.SaveChanges();
        }
    }
}
