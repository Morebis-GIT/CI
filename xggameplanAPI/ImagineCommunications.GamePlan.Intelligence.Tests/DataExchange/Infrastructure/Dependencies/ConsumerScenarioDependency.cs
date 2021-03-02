using System;
using System.Threading.Tasks;
using BoDi;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Implementation.GroupTransactions;
using ImagineCommunications.BusClient.Implementation.PayloadStorage;
using ImagineCommunications.BusClient.Implementation.Services;
using ImagineCommunications.BusClient.MassTransit;
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
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.ClashException;
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
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Extensions;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using MassTransit.Testing;
using Moq;
using xggameplan.common.Caching;
using xggameplan.core.Validations;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Dependencies
{
    public class ConsumerScenarioDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterInstanceAs(Mock.Of<IServiceProvider>());
            objectContainer.RegisterInstanceAs(Mock.Of<IObjectStorage>());
            objectContainer.RegisterTypeAs<JsonObjectSerializer, IObjectSerializer>();
            objectContainer.RegisterTypeAs<GroupTransactionExecutionService, IGroupTransactionExecutionService>();
            objectContainer.RegisterTypeAs<ClashExceptionValidations, IClashExceptionValidations>();

            objectContainer.RegisterInstanceAs<ICache>(new InMemoryCache());
            objectContainer.RegisterTypeAs<MessageTypeService, IMessageTypeService>();

            RegisterHandlers(objectContainer);

            InMemoryTestHarness bus = new InMemoryTestHarness();
            bus.AddConsumer<IGroupTransactionEvent>(objectContainer);
            bus.AddConsumer<IBulkClashCreatedOrUpdated>(objectContainer);
            bus.AddConsumer<IBulkClashDeleted>(objectContainer);
            bus.AddConsumer<IClashUpdated>(objectContainer);
            bus.AddConsumer<IClashTruncated>(objectContainer);

            bus.AddConsumer<IBulkBreakCreated>(objectContainer);
            bus.AddConsumer<IBulkBreaksDeleted>(objectContainer);

            bus.AddConsumer<IBulkProductCreatedOrUpdated>(objectContainer);
            bus.AddConsumer<IBulkProductDeleted>(objectContainer);

            bus.AddConsumer<IBulkHolidayCreated>(objectContainer);
            bus.AddConsumer<IBulkHolidayDeleted>(objectContainer);

            bus.AddConsumer<IDemographicUpdated>(objectContainer);
            bus.AddConsumer<IBulkDemographicDeleted>(objectContainer);
            bus.AddConsumer<IBulkDemographicCreatedOrUpdated>(objectContainer);

            bus.AddConsumer<IBulkProgrammeCreated>(objectContainer);
            bus.AddConsumer<IBulkProgrammeUpdated>(objectContainer);
            bus.AddConsumer<IBulkProgrammeDeleted>(objectContainer);

            bus.AddConsumer<IBulkProgrammeClassificationCreated>(objectContainer);
            bus.AddConsumer<IDeleteAllProgrammeClassification>(objectContainer);

            bus.AddConsumer<IBulkUniverseCreated>(objectContainer);
            bus.AddConsumer<IBulkUniverseDeleted>(objectContainer);

            bus.AddConsumer<IBulkRestrictionCreatedOrUpdated>(objectContainer);
            bus.AddConsumer<IBulkRestrictionDeleted>(objectContainer);

            bus.AddConsumer<IBulkSpotCreatedOrUpdated>(objectContainer);
            bus.AddConsumer<IBulkSpotDeleted>(objectContainer);

            bus.AddConsumer<IBulkRatingsPredictionSchedulesCreated>(objectContainer);
            bus.AddConsumer<IBulkRatingsPredictionSchedulesDeleted>(objectContainer);

            bus.AddConsumer<IBulkSalesAreaCreatedOrUpdated>(objectContainer);
            bus.AddConsumer<IBulkSalesAreaDeleted>(objectContainer);
            bus.AddConsumer<ISalesAreaUpdated>(objectContainer);

            bus.AddConsumer<IBulkClashExceptionCreated, BulkClashExceptionCreatedValidator>(objectContainer);
            bus.AddConsumer<IBulkClashExceptionDeleted, BulkClashExceptionDeletedValidator>(objectContainer);

            bus.AddConsumer<IBulkCampaignCreatedOrUpdated>(objectContainer);
            bus.AddConsumer<IBulkCampaignDeleted>(objectContainer);

            bus.AddConsumer<IBulkBookingPositionGroupCreated>(objectContainer);
            bus.AddConsumer<IBulkBookingPositionGroupDeleted>(objectContainer);
            bus.AddConsumer<IBookingPositionGroupTruncated>(objectContainer);

            bus.AddConsumer<IBulkProgrammeCategoryCreated>(objectContainer);
            bus.AddConsumer<IBulkProgrammeCategoryDeleted>(objectContainer);

            bus.AddConsumer<IBulkLockTypeCreated>(objectContainer);
            bus.AddConsumer<IBulkLockTypeDeleted>(objectContainer);

            bus.AddConsumer<IBulkInventoryTypeCreated>(objectContainer);
            bus.AddConsumer<IBulkInventoryTypeDeleted>(objectContainer);

            bus.AddConsumer<IBulkInventoryLockCreated>(objectContainer);
            bus.AddConsumer<IBulkInventoryLockDeleted>(objectContainer);

            bus.AddConsumer<IBulkTotalRatingCreated>(objectContainer);
            bus.AddConsumer<IBulkTotalRatingDeleted>(objectContainer);

            bus.AddConsumer<IBulkBreakTypeCreated>(objectContainer);
            bus.AddConsumer<IBulkBreakTypeDeleted>(objectContainer);

            bus.AddConsumer<IBulkStandardDayPartCreated>(objectContainer);
            bus.AddConsumer<IBulkStandardDayPartDeleted>(objectContainer);

            bus.AddConsumer<IBulkStandardDayPartGroupCreated>(objectContainer);
            bus.AddConsumer<IBulkStandardDayPartGroupDeleted>(objectContainer);

            bus.AddConsumer<IBulkSpotBookingRuleCreated>(objectContainer);
            bus.AddConsumer<IBulkSpotBookingRuleDeleted>(objectContainer);

            bus.AddConsumer<IBulkLengthFactorCreated>(objectContainer);
            bus.AddConsumer<IBulkLengthFactorDeleted>(objectContainer);

            Task.WaitAll(bus.Start());
            objectContainer.RegisterInstanceAs<InMemoryTestHarness>(bus);
            objectContainer.RegisterInstanceAs<IServiceBus>(new ServiceBus(
                bus.BusControl,
                Mock.Of<IContractValidatorService>(),
                objectContainer.Resolve<IBigMessageService>(),
                objectContainer.Resolve<ObjectStorageConfiguration>()));
        }

        private void RegisterHandlers(IObjectContainer objectContainer)
        {
            objectContainer.AddHandler<IBulkClashCreatedOrUpdated, BulkClashCreatedOrUpdatedEventHandler, BulkClashCreatedOrUpdated>();
            objectContainer.AddHandler<IBulkClashDeleted, BulkClashDeletedEventHandler, BulkClashDeleted>();
            objectContainer.AddHandler<IClashUpdated, ClashUpdatedEventHandler, ClashUpdated>();
            objectContainer.AddHandler<IClashTruncated, BulkClashTruncatedEventHandler, ClashTruncated>();

            objectContainer.AddHandler<IBulkBreakCreated, BulkBreakCreatedEventHandler, BulkBreakCreated>();
            objectContainer.AddHandler<IBulkBreaksDeleted, BulkBreaksDeletedEventHandler, BulkBreaksDeleted>();
            objectContainer.RegisterTypeAs<BulkBreakDeletedBatchingHandler, IBatchingHandler<IBulkBreaksDeleted>>();

            objectContainer.AddHandler<IBulkProductCreatedOrUpdated, SqlServerBulkProductCreatedOrUpdatedEventHandler, BulkProductCreatedOrUpdated>();
            objectContainer.AddHandler<IBulkProductDeleted, BulkProductDeletedEventHandler, BulkProductDeleted>();

            objectContainer.AddHandler<IBulkHolidayCreated, BulkHolidayCreatedEventHandler, BulkHolidayCreated>();
            objectContainer.AddHandler<IBulkHolidayDeleted, BulkHolidayDeletedEventHandler, BulkHolidayDeleted>();

            objectContainer.AddHandler<IBulkDemographicCreatedOrUpdated, BulkDemographicCreatedOrUpdatedEventHandler, BulkDemographicCreatedOrUpdated>();
            objectContainer.AddHandler<IDemographicUpdated, DemographicUpdatedEventHandler, DemographicUpdated>();
            objectContainer.AddHandler<IBulkDemographicDeleted, BulkDemographicDeletedEventHandler, BulkDemographicDeleted>();

            objectContainer.AddHandler<IBulkProgrammeCreated, BulkProgrammeCreatedEventHandler, BulkProgrammeCreated>();
            objectContainer.RegisterTypeAs<BulkProgrammeCreatedBatchingHandler, IBatchingHandler<IBulkProgrammeCreated>>();

            objectContainer.AddHandler<IBulkProgrammeUpdated, BulkProgrammeUpdatedEventHandler, BulkProgrammeUpdated>();
            objectContainer.AddHandler<IBulkProgrammeDeleted, BulkProgrammeDeletedEventHandler, BulkProgrammeDeleted>();
            objectContainer.RegisterTypeAs<BulkProgrammeDeletedBatchingHandler, IBatchingHandler<IBulkProgrammeDeleted>>();

            objectContainer.AddHandler<IBulkProgrammeClassificationCreated, BulkProgrammeClassificationCreatedEventHandler, BulkProgrammeClassificationCreated>();
            objectContainer.AddHandler<IDeleteAllProgrammeClassification, DeleteAllProgrammeClassificationEventHandler, DeleteAllProgrammeClassification>();

            objectContainer.AddHandler<IBulkUniverseCreated, BulkUniverseCreatedEventHandler, BulkUniverseCreated>();
            objectContainer.AddHandler<IBulkUniverseDeleted, BulkUniverseDeletedEventHandler, BulkUniverseDeleted>();

            objectContainer.AddHandler<IBulkRestrictionCreatedOrUpdated, BulkRestrictionCreatedOrUpdatedEventHandler, BulkRestrictionCreatedOrUpdated>();
            objectContainer.AddHandler<IBulkRestrictionDeleted, BulkRestrictionDeletedEventHandler, BulkRestrictionDeleted>();

            objectContainer.AddHandler<IBulkSpotCreatedOrUpdated, BulkSpotCreatedOrUpdatedEventHandler, BulkSpotCreatedOrUpdated>();
            objectContainer.AddHandler<IBulkSpotDeleted, BulkSpotDeletedEventHandler, BulkSpotDeleted>();

            objectContainer.AddHandler<IBulkRatingsPredictionSchedulesCreated, BulkRatingsPredictionSchedulesCreatedEventHandler, BulkRatingsPredictionScheduleCreated>();
            objectContainer.AddHandler<IBulkRatingsPredictionSchedulesDeleted, BulkRatingsPredictionSchedulesDeletedEventHandler, BulkRatingsPredictionSchedulesDeleted>();

            objectContainer.AddHandler<IBulkSalesAreaCreatedOrUpdated, BulkSalesAreaCreatedOrUpdatedHandler, BulkSalesAreaCreatedOrUpdated>();
            objectContainer.AddHandler<IBulkSalesAreaDeleted, BulkSalesAreaDeletedEventHandler, BulkSalesAreaDeleted>();
            objectContainer.AddHandler<ISalesAreaUpdated, SalesAreaUpdatedEventHandler, SalesAreaUpdated>();

            objectContainer.AddHandler<IBulkClashExceptionCreated, BulkClashExceptionCreatedEventHandler, BulkClashExceptionCreated>();
            objectContainer.AddHandler<IBulkClashExceptionDeleted, BulkClashExceptionDeletedEventHandler, BulkClashExceptionDeleted>();

            objectContainer.AddHandler<IBulkCampaignCreatedOrUpdated, SqlBulkCampaignCreatedOrUpdatedEventHandler, BulkCampaignCreatedOrUpdated>();
            objectContainer.RegisterTypeAs<CampaignCreatedOrUpdatedBatchingHandler, IBatchingHandler<IBulkCampaignCreatedOrUpdated>>();

            objectContainer.AddHandler<IBulkCampaignDeleted, BulkCampaignDeletedEventHandler, BulkCampaignDeleted>();

            objectContainer.AddHandler<IBulkBookingPositionGroupCreated, BulkBookingPositionGroupCreatedEventHandler, BulkBookingPositionGroupCreated>();
            objectContainer.AddHandler<IBulkBookingPositionGroupDeleted, BulkBookingPositionGroupDeletedEventHandler, BulkBookingPositionGroupDeleted>();
            objectContainer.AddHandler<IBookingPositionGroupTruncated, BookingPositionGroupTruncatedEventHandler, BookingPositionGroupTruncated>();

            objectContainer.AddHandler<IBulkProgrammeCategoryCreated, BulkProgrammeCategoryCreatedEventHandler, BulkProgrammeCategoryCreated>();
            objectContainer.AddHandler<IBulkProgrammeCategoryDeleted, BulkProgrammeCategoryDeletedEventHandler, BulkProgrammeCategoryDeleted>();

            objectContainer.AddHandler<IBulkLockTypeCreated, BulkLockTypeCreatedEventHandler, BulkLockTypeCreated>();
            objectContainer.AddHandler<IBulkLockTypeDeleted, BulkLockTypeDeletedEventHandler, BulkLockTypeDeleted>();

            objectContainer.AddHandler<IBulkInventoryTypeCreated, BulkInventoryTypeCreatedEventHandler, BulkInventoryTypeCreated>();
            objectContainer.AddHandler<IBulkInventoryTypeDeleted, BulkInventoryTypeDeletedEventHandler, BulkInventoryTypeDeleted>();

            objectContainer.AddHandler<IBulkInventoryLockCreated, BulkInventoryLockCreatedEventHandler, BulkInventoryLockCreated>();
            objectContainer.AddHandler<IBulkInventoryLockDeleted, BulkInventoryLockDeletedEventHandler, BulkInventoryLockDeleted>();

            objectContainer.AddHandler<IBulkTotalRatingCreated, BulkTotalRatingCreatedEventHandler, BulkTotalRatingCreated>();
            objectContainer.AddHandler<IBulkTotalRatingDeleted, BulkTotalRatingDeletedEventHandler, BulkTotalRatingDeleted>();

            objectContainer.AddHandler<IBulkBreakTypeCreated, BulkBreakTypeCreatedEventHandler, BulkBreakTypeCreated>();
            objectContainer.AddHandler<IBulkBreakTypeDeleted, BulkBreakTypeDeletedEventHandler, BulkBreakTypeDeleted>();

            objectContainer.AddHandler<IBulkStandardDayPartCreated, BulkStandardDayPartCreatedEventHandler, BulkStandardDayPartCreated>();
            objectContainer.AddHandler<IBulkStandardDayPartDeleted, BulkStandardDayPartDeletedEventHandler, BulkStandardDayPartDeleted>();

            objectContainer.AddHandler<IBulkStandardDayPartGroupCreated, BulkStandardDayPartGroupCreatedEventHandler, BulkStandardDayPartGroupCreated>();
            objectContainer.AddHandler<IBulkStandardDayPartGroupDeleted, BulkStandardDayPartGroupDeletedEventHandler, BulkStandardDayPartGroupDeleted>();

            objectContainer.AddHandler<IBulkSpotBookingRuleCreated, BulkSpotBookingRuleCreatedEventHandler, BulkSpotBookingRuleCreated>();
            objectContainer.AddHandler<IBulkSpotBookingRuleDeleted, BulkSpotBookingRuleDeletedEventHandler, BulkSpotBookingRuleDeleted>();

            objectContainer.AddHandler<IBulkLengthFactorCreated, BulkLengthFactorCreatedEventHandler, BulkLengthFactorCreated>();
            objectContainer.AddHandler<IBulkLengthFactorDeleted, BulkLengthFactorDeletedEventHandler, BulkLengthFactorDeleted>();

            _ = objectContainer.AddResolver<IHandlerDispatcher>();
            _ = objectContainer.AddResolver<IPayloadStorageProviderService>();
            objectContainer.RegisterTypeAs<PayloadStorageProviderService<IGroupTransactionEvent, GroupTransactionEvent>, IPayloadStorageProviderService>(nameof(IGroupTransactionEvent));
        }
    }
}
