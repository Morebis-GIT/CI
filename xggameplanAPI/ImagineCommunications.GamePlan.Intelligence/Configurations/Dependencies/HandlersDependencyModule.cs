using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Implementation.BigMessage;
using ImagineCommunications.BusClient.Implementation.PayloadStorage;
using ImagineCommunications.Extensions.DependencyInjection;
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
using ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Breaks;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.DayParts;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.GroupTransaction;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryLock;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryType;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.LockType;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.LengthFactor;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Product;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Spot;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Universe;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BookingPositionGroup;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Breaks;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Clash;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.DayParts;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Demographics;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Holidays;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.LengthFactor;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.LengthFactors;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Product;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Restriction;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SalesAreas;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Spot;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Universe;
using ImagineCommunications.GamePlan.Intelligence.Common;
using ImagineCommunications.GamePlan.Intelligence.Configurations.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Dependencies
{
    public class HandlersDependencyModule : IDependencyModule
    {
        public void Register(IServiceCollection services)
        {
            services.AddHandler<IBulkClashCreatedOrUpdated, BulkClashCreatedOrUpdatedEventHandler, BulkClashCreatedOrUpdated>();
            services.AddHandler<IBulkClashDeleted, BulkClashDeletedEventHandler, BulkClashDeleted>();
            services.AddHandler<IClashUpdated, ClashUpdatedEventHandler, ClashUpdated>();
            services.AddHandler<IClashTruncated, BulkClashTruncatedEventHandler, ClashTruncated>();

            services.AddHandler<IBulkBreakCreated, BulkBreakCreatedEventHandler, BulkBreakCreated>();
            services.AddHandler<IBulkBreaksDeleted, BulkBreaksDeletedEventHandler, BulkBreakDeletedBatchingHandler, BulkBreaksDeleted>();

            services.AddHandler<IBulkProductCreatedOrUpdated, SqlServerBulkProductCreatedOrUpdatedEventHandler, BulkProductCreatedOrUpdated>();
            services.AddHandler<IBulkProductDeleted, BulkProductDeletedEventHandler, BulkProductDeleted>();

            services.AddHandler<IBulkHolidayCreated, BulkHolidayCreatedEventHandler, BulkHolidayCreated>();
            services.AddHandler<IBulkHolidayDeleted, BulkHolidayDeletedEventHandler, BulkHolidayDeleted>();

            services.AddHandler<IBulkDemographicCreatedOrUpdated, BulkDemographicCreatedOrUpdatedEventHandler, BulkDemographicCreatedOrUpdated>();
            services.AddHandler<IDemographicUpdated, DemographicUpdatedEventHandler, DemographicUpdated>();
            services.AddHandler<IBulkDemographicDeleted, BulkDemographicDeletedEventHandler, BulkDemographicDeleted>();

            services.AddHandler<IBulkProgrammeCreated, BulkProgrammeCreatedEventHandler, BulkProgrammeCreatedBatchingHandler, BulkProgrammeCreated>();

            services.AddHandler<IBulkProgrammeUpdated, BulkProgrammeUpdatedEventHandler, BulkProgrammeUpdated>();
            services.AddHandler<IBulkProgrammeDeleted, BulkProgrammeDeletedEventHandler, BulkProgrammeDeletedBatchingHandler, BulkProgrammeDeleted>();

            services.AddHandler<IBulkProgrammeClassificationCreated, BulkProgrammeClassificationCreatedEventHandler, BulkProgrammeClassificationCreated>();
            services.AddHandler<IDeleteAllProgrammeClassification, DeleteAllProgrammeClassificationEventHandler, DeleteAllProgrammeClassification>();

            services.AddHandler<IBulkUniverseCreated, BulkUniverseCreatedEventHandler, BulkUniverseCreated>();
            services.AddHandler<IBulkUniverseDeleted, BulkUniverseDeletedEventHandler, BulkUniverseDeleted>();

            services.AddHandler<IBulkRestrictionCreatedOrUpdated, BulkRestrictionCreatedOrUpdatedEventHandler, BulkRestrictionCreatedOrUpdated>();
            services.AddHandler<IBulkRestrictionDeleted, BulkRestrictionDeletedEventHandler, BulkRestrictionDeleted>();

            services.AddHandler<IBulkSpotCreatedOrUpdated, BulkSpotCreatedOrUpdatedEventHandler, BulkSpotCreatedOrUpdated>();
            services.AddHandler<IBulkSpotDeleted, BulkSpotDeletedEventHandler, BulkSpotDeleted>();

            services.AddHandler<IBulkRatingsPredictionSchedulesCreated, BulkRatingsPredictionSchedulesCreatedEventHandler, BulkRatingsPredictionScheduleCreated>();
            services.AddHandler<IBulkRatingsPredictionSchedulesDeleted, BulkRatingsPredictionSchedulesDeletedEventHandler, BulkRatingsPredictionSchedulesDeleted>();

            services.AddHandler<IBulkSalesAreaCreatedOrUpdated, BulkSalesAreaCreatedOrUpdatedHandler, BulkSalesAreaCreatedOrUpdated>();
            services.AddHandler<IBulkSalesAreaDeleted, BulkSalesAreaDeletedEventHandler, BulkSalesAreaDeleted>();
            services.AddHandler<ISalesAreaUpdated, SalesAreaUpdatedEventHandler, SalesAreaUpdated>();

            services.AddHandler<IBulkClashExceptionCreated, BulkClashExceptionCreatedEventHandler, BulkClashExceptionCreated>();
            services.AddHandler<IBulkClashExceptionDeleted, BulkClashExceptionDeletedEventHandler, BulkClashExceptionDeleted>();

            services.AddHandler<IBulkCampaignCreatedOrUpdated, SqlBulkCampaignCreatedOrUpdatedEventHandler, CampaignCreatedOrUpdatedBatchingHandler, BulkCampaignCreatedOrUpdated>();

            services.AddHandler<IBulkCampaignDeleted, BulkCampaignDeletedEventHandler, BulkCampaignDeleted>();

            services.AddHandler<IBulkBookingPositionGroupCreated, BulkBookingPositionGroupCreatedEventHandler, BulkBookingPositionGroupCreated>();
            services.AddHandler<IBulkBookingPositionGroupDeleted, BulkBookingPositionGroupDeletedEventHandler, BulkBookingPositionGroupDeleted>();
            services.AddHandler<IBookingPositionGroupTruncated, BookingPositionGroupTruncatedEventHandler, BookingPositionGroupTruncated>();

            services.AddHandler<IBulkProgrammeCategoryCreated, BulkProgrammeCategoryCreatedEventHandler, BulkProgrammeCategoryCreated>();
            services.AddHandler<IBulkProgrammeCategoryDeleted, BulkProgrammeCategoryDeletedEventHandler, BulkProgrammeCategoryDeleted>();

            services.AddHandler<IBulkLockTypeCreated, BulkLockTypeCreatedEventHandler, BulkLockTypeCreated>();
            services.AddHandler<IBulkLockTypeDeleted, BulkLockTypeDeletedEventHandler, BulkLockTypeDeleted>();

            services.AddHandler<IBulkInventoryTypeCreated, BulkInventoryTypeCreatedEventHandler, BulkInventoryTypeCreated>();
            services.AddHandler<IBulkInventoryTypeDeleted, BulkInventoryTypeDeletedEventHandler, BulkInventoryTypeDeleted>();

            services.AddHandler<IBulkInventoryLockCreated, BulkInventoryLockCreatedEventHandler, BulkInventoryLockCreated>();
            services.AddHandler<IBulkInventoryLockDeleted, BulkInventoryLockDeletedEventHandler, BulkInventoryLockDeleted>();

            services.AddHandler<IBulkTotalRatingCreated, BulkTotalRatingCreatedEventHandler, BulkTotalRatingCreated>();
            services.AddHandler<IBulkTotalRatingDeleted, BulkTotalRatingDeletedEventHandler, BulkTotalRatingDeleted>();

            services.AddHandler<IBulkBreakTypeCreated, BulkBreakTypeCreatedEventHandler, BulkBreakTypeCreated>();
            services.AddHandler<IBulkBreakTypeDeleted, BulkBreakTypeDeletedEventHandler, BulkBreakTypeDeleted>();

            services.AddTransient<IPayloadStorageProviderService, PayloadStorageProviderService<IBigMessage, BigMessage>>(nameof(IBigMessage));
            services.AddTransient<IPayloadStorageProviderService, PayloadStorageProviderService<IGroupTransactionEvent, GroupTransactionEvent>>(nameof(IGroupTransactionEvent));

            services.AddHandler<IBulkStandardDayPartCreated, BulkStandardDayPartCreatedEventHandler, BulkStandardDayPartCreated>();
            services.AddHandler<IBulkStandardDayPartDeleted, BulkStandardDayPartDeletedEventHandler, BulkStandardDayPartDeleted>();

            services.AddHandler<IBulkStandardDayPartGroupCreated, BulkStandardDayPartGroupCreatedEventHandler, BulkStandardDayPartGroupCreated>();
            services.AddHandler<IBulkStandardDayPartGroupDeleted, BulkStandardDayPartGroupDeletedEventHandler, BulkStandardDayPartGroupDeleted>();

            services.AddHandler<IBulkLengthFactorCreated, BulkLengthFactorCreatedEventHandler, BulkLengthFactorCreated>();
            services.AddHandler<IBulkLengthFactorDeleted, BulkLengthFactorDeletedEventHandler, BulkLengthFactorDeleted>();

            services.AddTransientWithKey<IPayloadStorageProviderService, PayloadStorageProviderService<IGroupTransactionEvent, GroupTransactionEvent>>(nameof(IGroupTransactionEvent));
            services.AddTransientWithKey<IPayloadStorageProviderService, PayloadStorageProviderService<IBigMessage, BigMessage>>(nameof(IBigMessage));

            services.AddHandler<IBulkSpotBookingRuleCreated, BulkSpotBookingRuleCreatedEventHandler, BulkSpotBookingRuleCreated>();
            services.AddHandler<IBulkSpotBookingRuleDeleted, BulkSpotBookingRuleDeletedEventHandler, BulkSpotBookingRuleDeleted>();

            services.AddResolver<IPayloadStorageProviderService>();
            services.AddResolver<IHandlerDispatcher>();
            services.AddScoped<IBulkCampaignCreatedOrUpdatedEventHandler, SqlBulkCampaignCreatedOrUpdatedEventHandler>();
        }
    }
}
