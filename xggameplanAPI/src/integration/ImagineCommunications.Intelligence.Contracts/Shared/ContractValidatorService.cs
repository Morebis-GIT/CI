using System;
using System.Collections.Generic;
using FluentValidation;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.BookingPositionGroup;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Breaks;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Clash;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.ClashException;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.DayParts;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Demographics;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.GroupTransaction;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.InventoryLock;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.InventoryType;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.LockType;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.LengthFactors;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Product;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Programme;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.SalesArea;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Spot;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Shared
{
    public class ContractValidatorService : IContractValidatorService
    {
        private static Dictionary<Type, Func<IValidator>> contractValidatorMapping { get; set; }
        static ContractValidatorService()
        {
            contractValidatorMapping = new Dictionary<Type, Func<IValidator>>();

            contractValidatorMapping.Add(typeof(IBulkClashCreatedOrUpdated), () => new BulkClashCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkClashDeleted), () => new BulkClashDeletedValidator());
            contractValidatorMapping.Add(typeof(IClashUpdated), () => new ClashUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkUniverseCreated), () => new BulkUniverseCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkUniverseDeleted), () => new BulkUniverseDeletedValidator());
            contractValidatorMapping.Add(typeof(IDemographicUpdated), () => new DemographicUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkDemographicCreatedOrUpdated), () => new BulkDemographicCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkDemographicDeleted), () => new BulkDemographicDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkRatingsPredictionSchedulesCreated), () => new BulkRatingsPredictionSchedulesCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkRatingsPredictionSchedulesDeleted), () => new BulkRatingsPredictionSchedulesDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkSalesAreaCreatedOrUpdated), () => new BulkSalesAreaCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(ISalesAreaUpdated), () => new SalesAreaUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkSalesAreaDeleted), () => new BulkSalesAreaDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkBreakCreated), () => new BulkBreakCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkBreaksDeleted), () => new BulkBreakDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkClashExceptionCreated), () => new BulkClashExceptionCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkClashExceptionDeleted), () => new BulkClashExceptionDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkProgrammeCreated), () => new BulkProgrammeCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkProgrammeUpdated), () => new BulkProgrammeUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkProgrammeDeleted), () => new BulkProgrammeDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkHolidayCreated), () => new BulkHolidayCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkHolidayDeleted), () => new BulkHolidayDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkProgrammeClassificationCreated), () => new BulkProgrammeClassificationCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkProductDeleted), () => new BulkProductDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkProductCreatedOrUpdated), () => new BulkProductCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkRestrictionCreatedOrUpdated), () => new BulkRestrictionCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkRestrictionDeleted), () => new BulkRestrictionDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkSpotCreatedOrUpdated), () => new BulkSpotCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkSpotDeleted), () => new BulkSpotDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkCampaignCreatedOrUpdated), () => new BulkCampaignCreatedOrUpdatedValidator());
            contractValidatorMapping.Add(typeof(IBulkCampaignDeleted), () => new BulkCampaignDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkBookingPositionGroupCreated), () => new BulkBookingPositionGroupCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkBookingPositionGroupDeleted), () => new BulkBookingPositionGroupDeletedValidator());
            contractValidatorMapping.Add(typeof(IProgrammeCategoryCreated), () => new ProgrammeCategoryCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkProgrammeCategoryCreated), () => new BulkProgrammeCategoryCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkLockTypeCreated), () => new BulkLockTypeCreatedValidator());
            contractValidatorMapping.Add(typeof(ILockTypeCreated), () => new LockTypeCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkInventoryTypeCreated), () => new BulkInventoryTypeCreatedValidator());
            contractValidatorMapping.Add(typeof(IInventoryTypeCreated), () => new InventoryTypeCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkInventoryLockCreated), () => new BulkInventoryLockCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkInventoryLockDeleted), () => new BulkInventoryLockDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkTotalRatingCreated), () => new BulkTotalRatingCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkTotalRatingDeleted), () => new BulkTotalRatingDeletedValidator());
            contractValidatorMapping.Add(typeof(IBulkBreakTypeCreated), () => new BulkBreakTypeCreatedValidator());
            contractValidatorMapping.Add(typeof(IGroupTransactionEvent), () => new GroupTransactionEventValidator());
            contractValidatorMapping.Add(typeof(IBulkStandardDayPartCreated), () => new BulkStandardDayPartCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkStandardDayPartGroupCreated), () => new BulkStandardDayPartGroupCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkSpotBookingRuleCreated), () => new BulkSpotBookingRuleCreatedValidator());
            contractValidatorMapping.Add(typeof(IBulkLengthFactorCreated), () => new BulkLengthFactorCreatedValidator());
        }

        public void Validate<T>(T contract)
        {
            Func<IValidator> validatorConstructor = null;
            contractValidatorMapping.TryGetValue(typeof(T), out validatorConstructor);
            if (validatorConstructor != null)
            {
                var validationResult = validatorConstructor().Validate(contract);
                if (!validationResult.IsValid)
                {
                    throw new ContractValidationException(validationResult.Errors);
                }
            }
        }
    }
}
