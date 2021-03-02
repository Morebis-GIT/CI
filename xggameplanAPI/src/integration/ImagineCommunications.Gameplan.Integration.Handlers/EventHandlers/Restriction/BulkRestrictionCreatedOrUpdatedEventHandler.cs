using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using RestrictionDbObject = ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Restriction
{
    public class BulkRestrictionCreatedOrUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkRestrictionCreatedOrUpdated>
    {
        private readonly IMapper _mapper;
        private readonly IRestrictionRepository _restrictionRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IClearanceRepository _clearanceRepository;
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;

        public BulkRestrictionCreatedOrUpdatedEventHandler(
            IRestrictionRepository restrictionRepository,
            ISalesAreaRepository salesAreaRepository,
            IClearanceRepository clearanceRepository,
            IProgrammeCategoryHierarchyRepository programmeCategoryRepository,
            IMapper mapper)
        {
            _restrictionRepository = restrictionRepository;
            _salesAreaRepository = salesAreaRepository;
            _clearanceRepository = clearanceRepository;
            _programmeCategoryRepository = programmeCategoryRepository;
            _mapper = mapper;
        }

        public override void Handle(IBulkRestrictionCreatedOrUpdated command)
        {
            if (command.Data.Any())
            {
                var newRestrictionModelItems = command.Data.ToList();

                ValidateNewRestrictions(newRestrictionModelItems);

                var salesAreas = newRestrictionModelItems.SelectMany(x => x.SalesAreas)
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();

                _salesAreaRepository.ValidateSalesArea(salesAreas);

                var programmeCategoryNames = newRestrictionModelItems
                    .Select(x => x.ProgrammeCategory)
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                _programmeCategoryRepository.ValidateProgrammeCategory(programmeCategoryNames);

                var clearenceCodes = newRestrictionModelItems
                    .Where(x => !string.IsNullOrWhiteSpace(x.ClearanceCode))
                    .Select(x => x.ClearanceCode)
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                ValidateClearanceCodes(clearenceCodes);

                var newRestrictions = _mapper.Map<List<RestrictionDbObject>>(newRestrictionModelItems);

                var allSalesAreaQty = _salesAreaRepository.CountAll;
                foreach (var newRestriction in newRestrictions)
                {
                    newRestriction.Uid = Guid.NewGuid();

                    if ((newRestriction.SalesAreas?.Any() ?? false) &&
                    newRestriction.SalesAreas.Count == allSalesAreaQty)
                    {
                        newRestriction.SalesAreas = new List<string>();
                    }
                }

                _restrictionRepository.UpdateRange(newRestrictions);
                _restrictionRepository.SaveChanges();
            }
        }

        private static void ValidateNewRestrictions(IEnumerable<IRestrictionCreatedOrUpdated> newItems)
        {
            var bulkCreatedCommand = new BulkRestrictionCreatedOrUpdated(newItems.Select(x => (RestrictionCreatedOrUpdated)x));
            var result = new BulkRestrictionCreatedValidator().Validate(bulkCreatedCommand);

            if (!result.IsValid)
            {
                throw new ContractValidationException(result.Errors);
            }
        }

        private void ValidateClearanceCodes(List<string> clearanceCodes)
        {
            var existingClearanceCodes = _clearanceRepository.FindByExternal(clearanceCodes)
                    .Select(x => x.Code).ToList();

            var invalidClearanceCodes = clearanceCodes.Where(x => !existingClearanceCodes.Contains(x)).ToList();

            if (invalidClearanceCodes.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.ClearanceCodeNotFound,
                    $"Invalid clearance codes: {string.Join(", ", invalidClearanceCodes)}");
            }
        }

    }
}
