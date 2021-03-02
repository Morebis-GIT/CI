using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Implementation.GroupTransactions;
using ImagineCommunications.BusClient.MassTransit;
using ImagineCommunications.BusClient.MassTransit.Decorators;
using ImagineCommunications.BusClient.MassTransit.DiExtensions;
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
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations
{
    public static class ServiceBusConfigurator
    {
        public static void Init(IServiceCollection serviceCollection, ServiceBusConfigModel messagingConfig)
        {
            serviceCollection.AddScoped(typeof(EventConsumer<>), typeof(EventConsumer<>));
            serviceCollection.AddScoped(typeof(CommandConsumer<>), typeof(CommandConsumer<>));
            serviceCollection.AddSingleton<IServiceBus, ServiceBus>();
            serviceCollection.AddMassTransit(messagingConfig, ConsumerConfig, null);
            serviceCollection.AddScoped<IGroupTransactionExecutionService, GroupTransactionExecutionService>();
        }

        private static readonly ServiceBusConsumerConfigModel ConsumerConfig = new ServiceBusConsumerConfigModel
        {
            Consumers = new List<ServiceBusConsumerConfigModel.Consumer>
            {
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IGroupTransactionEvent),
                    ConsumerType = typeof(EventConsumer<IGroupTransactionEvent>),
                    QueueName = nameof(IGroupTransactionEvent),
                    ExchangeName = nameof(IGroupTransactionEvent)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBigMessage),
                    ConsumerType = typeof(EventConsumer<IBigMessage>),
                    QueueName = nameof(IBigMessage),
                    ExchangeName = nameof(IBigMessage)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IDemographicUpdated),
                    ConsumerType = typeof(EventConsumer<IDemographicUpdated>),
                    QueueName = nameof(IDemographicUpdated),
                    ExchangeName = nameof(IDemographicUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkDemographicCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkDemographicCreatedOrUpdated>),
                    QueueName = nameof(IBulkDemographicCreatedOrUpdated),
                    ExchangeName = nameof(IBulkDemographicCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkDemographicDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkDemographicDeleted>),
                    QueueName = nameof(IBulkDemographicDeleted),
                    ExchangeName = nameof(IBulkDemographicDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkUniverseCreated),
                    ConsumerType = typeof(EventConsumer<IBulkUniverseCreated>),
                    QueueName = nameof(IBulkUniverseCreated),
                    ExchangeName = nameof(IBulkUniverseCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkUniverseDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkUniverseDeleted>),
                    QueueName = nameof(IBulkUniverseDeleted),
                    ExchangeName = nameof(IBulkUniverseDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProgrammeCreated),
                    ConsumerType = typeof(EventConsumer<IBulkProgrammeCreated>),
                    QueueName = nameof(IBulkProgrammeCreated),
                    ExchangeName = nameof(IBulkProgrammeCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProgrammeUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkProgrammeUpdated>),
                    QueueName = nameof(IBulkProgrammeUpdated),
                    ExchangeName = nameof(IBulkProgrammeUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProgrammeDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkProgrammeDeleted>),
                    QueueName = nameof(IBulkProgrammeDeleted),
                    ExchangeName = nameof(IBulkProgrammeDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkHolidayCreated),
                    ConsumerType = typeof(EventConsumer<IBulkHolidayCreated>),
                    QueueName = nameof(IBulkHolidayCreated),
                    ExchangeName = nameof(IBulkHolidayCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkHolidayDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkHolidayDeleted>),
                    QueueName = nameof(IBulkHolidayDeleted),
                    ExchangeName = nameof(IBulkHolidayDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProgrammeClassificationCreated),
                    ConsumerType = typeof(EventConsumer<IBulkProgrammeClassificationCreated>),
                    QueueName = nameof(IBulkProgrammeClassificationCreated),
                    ExchangeName = nameof(IBulkProgrammeClassificationCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IDeleteAllProgrammeClassification),
                    ConsumerType = typeof(EventConsumer<IDeleteAllProgrammeClassification>),
                    QueueName = nameof(IDeleteAllProgrammeClassification),
                    ExchangeName = nameof(IDeleteAllProgrammeClassification)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkClashCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkClashCreatedOrUpdated>),
                    QueueName = nameof(IBulkClashCreatedOrUpdated),
                    ExchangeName = nameof(IBulkClashCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IClashUpdated),
                    ConsumerType = typeof(EventConsumer<IClashUpdated>),
                    QueueName = nameof(IClashUpdated),
                    ExchangeName = nameof(IClashUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkClashDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkClashDeleted>),
                    QueueName = nameof(IBulkClashDeleted),
                    ExchangeName = nameof(IBulkClashDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IClashTruncated),
                    ConsumerType = typeof(EventConsumer<IClashTruncated>),
                    QueueName = nameof(IClashTruncated),
                    ExchangeName = nameof(IClashTruncated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkRatingsPredictionSchedulesCreated),
                    ConsumerType = typeof(EventConsumer<IBulkRatingsPredictionSchedulesCreated>),
                    QueueName = nameof(IBulkRatingsPredictionSchedulesCreated),
                    ExchangeName = nameof(IBulkRatingsPredictionSchedulesCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkRatingsPredictionSchedulesDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkRatingsPredictionSchedulesDeleted>),
                    QueueName = nameof(IBulkRatingsPredictionSchedulesDeleted),
                    ExchangeName = nameof(IBulkRatingsPredictionSchedulesDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProductCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkProductCreatedOrUpdated>),
                    QueueName = nameof(IBulkProductCreatedOrUpdated),
                    ExchangeName = nameof(IBulkProductCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProductDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkProductDeleted>),
                    QueueName = nameof(IBulkProductDeleted),
                    ExchangeName = nameof(IBulkProductDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkSalesAreaCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkSalesAreaCreatedOrUpdated>),
                    QueueName = nameof(IBulkSalesAreaCreatedOrUpdated),
                    ExchangeName = nameof(IBulkSalesAreaCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(ISalesAreaUpdated),
                    ConsumerType = typeof(EventConsumer<ISalesAreaUpdated>),
                    QueueName = nameof(ISalesAreaUpdated),
                    ExchangeName = nameof(ISalesAreaUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkSalesAreaDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkSalesAreaDeleted>),
                    QueueName = nameof(IBulkSalesAreaDeleted),
                    ExchangeName = nameof(IBulkSalesAreaDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkRestrictionCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkRestrictionCreatedOrUpdated>),
                    QueueName = nameof(IBulkRestrictionCreatedOrUpdated),
                    ExchangeName = nameof(IBulkRestrictionCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkRestrictionDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkRestrictionDeleted>),
                    QueueName = nameof(IBulkRestrictionDeleted),
                    ExchangeName = nameof(IBulkRestrictionDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkSpotCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkSpotCreatedOrUpdated>),
                    QueueName = nameof(IBulkSpotCreatedOrUpdated),
                    ExchangeName = nameof(IBulkSpotCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkSpotDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkSpotDeleted>),
                    QueueName = nameof(IBulkSpotDeleted),
                    ExchangeName = nameof(IBulkSpotDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkBreakCreated),
                    ConsumerType = typeof(EventConsumer<IBulkBreakCreated>),
                    QueueName = nameof(IBulkBreakCreated),
                    ExchangeName = nameof(IBulkBreakCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkBreaksDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkBreaksDeleted>),
                    QueueName = nameof(IBulkBreaksDeleted),
                    ExchangeName = nameof(IBulkBreaksDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkClashExceptionDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkClashExceptionDeleted>),
                    QueueName = nameof(IBulkClashExceptionDeleted),
                    ExchangeName = nameof(IBulkClashExceptionDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkClashExceptionCreated),
                    ConsumerType = typeof(EventConsumer<IBulkClashExceptionCreated>),
                    QueueName = nameof(IBulkClashExceptionCreated),
                    ExchangeName = nameof(IBulkClashExceptionCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkCampaignCreatedOrUpdated),
                    ConsumerType = typeof(EventConsumer<IBulkCampaignCreatedOrUpdated>),
                    QueueName = nameof(IBulkCampaignCreatedOrUpdated),
                    ExchangeName = nameof(IBulkCampaignCreatedOrUpdated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkCampaignDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkCampaignDeleted>),
                    QueueName = nameof(IBulkCampaignDeleted),
                    ExchangeName = nameof(IBulkCampaignDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkBookingPositionGroupCreated),
                    ConsumerType = typeof(EventConsumer<IBulkBookingPositionGroupCreated>),
                    QueueName = nameof(IBulkBookingPositionGroupCreated),
                    ExchangeName = nameof(IBulkBookingPositionGroupCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkBookingPositionGroupDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkBookingPositionGroupDeleted>),
                    QueueName = nameof(IBulkBookingPositionGroupDeleted),
                    ExchangeName = nameof(IBulkBookingPositionGroupDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBookingPositionGroupTruncated),
                    ConsumerType = typeof(EventConsumer<IBookingPositionGroupTruncated>),
                    QueueName = nameof(IBookingPositionGroupTruncated),
                    ExchangeName = nameof(IBookingPositionGroupTruncated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProgrammeCategoryCreated),
                    ConsumerType = typeof(EventConsumer<IBulkProgrammeCategoryCreated>),
                    QueueName = nameof(IBulkProgrammeCategoryCreated),
                    ExchangeName = nameof(IBulkProgrammeCategoryCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkProgrammeCategoryDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkProgrammeCategoryDeleted>),
                    QueueName = nameof(IBulkProgrammeCategoryDeleted),
                    ExchangeName = nameof(IBulkProgrammeCategoryDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkLockTypeCreated),
                    ConsumerType = typeof(EventConsumer<IBulkLockTypeCreated>),
                    QueueName = nameof(IBulkLockTypeCreated),
                    ExchangeName = nameof(IBulkLockTypeCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkLockTypeDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkLockTypeDeleted>),
                    QueueName = nameof(IBulkLockTypeDeleted),
                    ExchangeName = nameof(IBulkLockTypeDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkInventoryTypeCreated),
                    ConsumerType = typeof(EventConsumer<IBulkInventoryTypeCreated>),
                    QueueName = nameof(IBulkInventoryTypeCreated),
                    ExchangeName = nameof(IBulkInventoryTypeCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkInventoryTypeDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkInventoryTypeDeleted>),
                    QueueName = nameof(IBulkInventoryTypeDeleted),
                    ExchangeName = nameof(IBulkInventoryTypeDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkInventoryLockCreated),
                    ConsumerType = typeof(EventConsumer<IBulkInventoryLockCreated>),
                    QueueName = nameof(IBulkInventoryLockCreated),
                    ExchangeName = nameof(IBulkInventoryLockCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkInventoryLockDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkInventoryLockDeleted>),
                    QueueName = nameof(IBulkInventoryLockDeleted),
                    ExchangeName = nameof(IBulkInventoryLockDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkTotalRatingCreated),
                    ConsumerType = typeof(EventConsumer<IBulkTotalRatingCreated>),
                    QueueName = nameof(IBulkTotalRatingCreated),
                    ExchangeName = nameof(IBulkTotalRatingCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkTotalRatingDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkTotalRatingDeleted>),
                    QueueName = nameof(IBulkTotalRatingDeleted),
                    ExchangeName = nameof(IBulkTotalRatingDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkBreakTypeCreated),
                    ConsumerType = typeof(EventConsumer<IBulkBreakTypeCreated>),
                    QueueName = nameof(IBulkBreakTypeCreated),
                    ExchangeName = nameof(IBulkBreakTypeCreated),
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkBreakTypeDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkBreakTypeDeleted>),
                    QueueName = nameof(IBulkBreakTypeDeleted),
                    ExchangeName = nameof(IBulkBreakTypeDeleted),
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkStandardDayPartCreated),
                    ConsumerType = typeof(EventConsumer<IBulkStandardDayPartCreated>),
                    QueueName = nameof(IBulkStandardDayPartCreated),
                    ExchangeName = nameof(IBulkStandardDayPartCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkStandardDayPartDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkStandardDayPartDeleted>),
                    QueueName = nameof(IBulkStandardDayPartDeleted),
                    ExchangeName = nameof(IBulkStandardDayPartDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkStandardDayPartGroupCreated),
                    ConsumerType = typeof(EventConsumer<IBulkStandardDayPartGroupCreated>),
                    QueueName = nameof(IBulkStandardDayPartGroupCreated),
                    ExchangeName = nameof(IBulkStandardDayPartGroupCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkStandardDayPartGroupDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkStandardDayPartGroupDeleted>),
                    QueueName = nameof(IBulkStandardDayPartGroupDeleted),
                    ExchangeName = nameof(IBulkStandardDayPartGroupDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkSpotBookingRuleCreated),
                    ConsumerType = typeof(EventConsumer<IBulkSpotBookingRuleCreated>),
                    QueueName = nameof(IBulkSpotBookingRuleCreated),
                    ExchangeName = nameof(IBulkSpotBookingRuleCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkSpotBookingRuleDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkSpotBookingRuleDeleted>),
                    QueueName = nameof(IBulkSpotBookingRuleDeleted),
                    ExchangeName = nameof(IBulkSpotBookingRuleDeleted)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkLengthFactorCreated),
                    ConsumerType = typeof(EventConsumer<IBulkLengthFactorCreated>),
                    QueueName = nameof(IBulkLengthFactorCreated),
                    ExchangeName = nameof(IBulkLengthFactorCreated)
                },
                new ServiceBusConsumerConfigModel.Consumer
                {
                    EntityType = typeof(IBulkLengthFactorDeleted),
                    ConsumerType = typeof(EventConsumer<IBulkLengthFactorDeleted>),
                    QueueName = nameof(IBulkLengthFactorDeleted),
                    ExchangeName = nameof(IBulkLengthFactorDeleted)
                }
            }
        };
    }
}
