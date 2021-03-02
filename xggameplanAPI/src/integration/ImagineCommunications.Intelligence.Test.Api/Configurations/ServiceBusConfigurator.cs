using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
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

namespace ImagineCommunications.Intelligence.Test.Api
{
    public static class ServiceBusConfigurator
    {
        public static void Init(IServiceCollection serviceCollection, ServiceBusConfigModel messagingConfig)
        {
            serviceCollection.AddMassTransit(messagingConfig, null, ProducerConfig);
        }

        private static readonly ServiceBusProducerConfigModel ProducerConfig = new ServiceBusProducerConfigModel
        {
            Producers = new List<ServiceBusProducerConfigModel.Producer>
            {
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IGroupTransactionEvent),
                    QueueName = nameof(IGroupTransactionEvent),
                    ExchangeName = nameof(IGroupTransactionEvent)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBigMessage),
                    QueueName = nameof(IBigMessage),
                    ExchangeName = nameof(IBigMessage)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkUniverseCreated),
                    QueueName = nameof(IBulkUniverseCreated),
                    ExchangeName = nameof(IBulkUniverseCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkUniverseDeleted),
                    QueueName = nameof(IBulkUniverseDeleted),
                    ExchangeName = nameof(IBulkUniverseDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IDemographicUpdated),
                    QueueName = nameof(IDemographicUpdated),
                    ExchangeName = nameof(IDemographicUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkDemographicCreatedOrUpdated),
                    QueueName = nameof(IBulkDemographicCreatedOrUpdated),
                    ExchangeName = nameof(IBulkDemographicCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkDemographicDeleted),
                    QueueName = nameof(IBulkDemographicDeleted),
                    ExchangeName = nameof(IBulkDemographicDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkProgrammeCreated),
                    QueueName = nameof(IBulkProgrammeCreated),
                    ExchangeName = nameof(IBulkProgrammeCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkProgrammeUpdated),
                    QueueName = nameof(IBulkProgrammeUpdated),
                    ExchangeName = nameof(IBulkProgrammeUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkProgrammeDeleted),
                    QueueName = nameof(IBulkProgrammeDeleted),
                    ExchangeName = nameof(IBulkProgrammeDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkHolidayCreated),
                    QueueName = nameof(IBulkHolidayCreated),
                    ExchangeName = nameof(IBulkHolidayCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkHolidayDeleted),
                    QueueName = nameof(IBulkHolidayDeleted),
                    ExchangeName = nameof(IBulkHolidayDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkProgrammeClassificationCreated),
                    QueueName = nameof(IBulkProgrammeClassificationCreated),
                    ExchangeName = nameof(IBulkProgrammeClassificationCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IDeleteAllProgrammeClassification),
                    QueueName = nameof(IDeleteAllProgrammeClassification),
                    ExchangeName = nameof(IDeleteAllProgrammeClassification)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkClashCreatedOrUpdated),
                    QueueName = nameof(IBulkClashCreatedOrUpdated),
                    ExchangeName = nameof(IBulkClashCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkClashDeleted),
                    QueueName = nameof(IBulkClashDeleted),
                    ExchangeName = nameof(IBulkClashDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IClashTruncated),
                    QueueName = nameof(IClashTruncated),
                    ExchangeName = nameof(IClashTruncated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkProductCreatedOrUpdated),
                    QueueName = nameof(IBulkProductCreatedOrUpdated),
                    ExchangeName = nameof(IBulkProductCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkProductDeleted),
                    QueueName = nameof(IBulkProductDeleted),
                    ExchangeName = nameof(IBulkProductDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkRatingsPredictionSchedulesDeleted),
                    QueueName= nameof(IBulkRatingsPredictionSchedulesDeleted),
                    ExchangeName = nameof(IBulkRatingsPredictionSchedulesDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkRatingsPredictionSchedulesCreated),
                    QueueName = nameof(IBulkRatingsPredictionSchedulesCreated),
                    ExchangeName = nameof(IBulkRatingsPredictionSchedulesCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkSalesAreaCreatedOrUpdated),
                    QueueName = nameof(IBulkSalesAreaCreatedOrUpdated),
                    ExchangeName =nameof(IBulkSalesAreaCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(ISalesAreaUpdated),
                    QueueName = nameof(ISalesAreaUpdated),
                    ExchangeName =nameof(ISalesAreaUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkSalesAreaDeleted),
                    QueueName = nameof(IBulkSalesAreaDeleted),
                    ExchangeName =nameof(IBulkSalesAreaDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkRestrictionCreatedOrUpdated),
                    QueueName = nameof(IBulkRestrictionCreatedOrUpdated),
                    ExchangeName = nameof(IBulkRestrictionCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkRestrictionDeleted),
                    QueueName = nameof(IBulkRestrictionDeleted),
                    ExchangeName = nameof(IBulkRestrictionDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkSpotCreatedOrUpdated),
                    QueueName = nameof(IBulkSpotCreatedOrUpdated),
                    ExchangeName = nameof(IBulkSpotCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkSpotDeleted),
                    QueueName = nameof(IBulkSpotDeleted),
                    ExchangeName = nameof(IBulkSpotDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkBreakCreated),
                    QueueName = nameof(IBulkBreakCreated),
                    ExchangeName = nameof(IBulkBreakCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkBreaksDeleted),
                    QueueName = nameof(IBulkBreaksDeleted),
                    ExchangeName = nameof(IBulkBreaksDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkClashExceptionCreated),
                    ExchangeName = nameof(IBulkClashExceptionCreated),
                    QueueName = nameof(IBulkClashExceptionCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkClashExceptionDeleted),
                    ExchangeName = nameof(IBulkClashExceptionDeleted),
                    QueueName = nameof(IBulkClashExceptionDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkCampaignCreatedOrUpdated),
                    ExchangeName = nameof(IBulkCampaignCreatedOrUpdated),
                    QueueName = nameof(IBulkCampaignCreatedOrUpdated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkCampaignDeleted),
                    ExchangeName = nameof(IBulkCampaignDeleted),
                    QueueName=nameof(IBulkCampaignDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkBookingPositionGroupCreated),
                    QueueName = nameof(IBulkBookingPositionGroupCreated),
                    ExchangeName = nameof(IBulkBookingPositionGroupCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkBookingPositionGroupDeleted),
                    QueueName = nameof(IBulkBookingPositionGroupDeleted),
                    ExchangeName = nameof(IBulkBookingPositionGroupDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType= typeof(IBulkProgrammeCategoryCreated),
                    ExchangeName= nameof(IBulkProgrammeCategoryCreated),
                    QueueName=nameof(IBulkProgrammeCategoryCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType= typeof(IBulkProgrammeCategoryDeleted),
                    ExchangeName= nameof(IBulkProgrammeCategoryDeleted),
                    QueueName=nameof(IBulkProgrammeCategoryDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkLockTypeCreated),
                    QueueName = nameof(IBulkLockTypeCreated),
                    ExchangeName = nameof(IBulkLockTypeCreated)
                },
                new ServiceBusProducerConfigModel.Producer

                {
                    EntityType = typeof(IBulkLockTypeDeleted),
                    QueueName = nameof(IBulkLockTypeDeleted),
                    ExchangeName = nameof(IBulkLockTypeDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkInventoryTypeCreated),
                    QueueName = nameof(IBulkInventoryTypeCreated),
                    ExchangeName = nameof(IBulkInventoryTypeCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkInventoryTypeDeleted),
                    QueueName = nameof(IBulkInventoryTypeDeleted),
                    ExchangeName = nameof(IBulkInventoryTypeDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkInventoryLockCreated),
                    QueueName = nameof(IBulkInventoryLockCreated),
                    ExchangeName = nameof(IBulkInventoryLockCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkInventoryLockDeleted),
                    QueueName = nameof(IBulkInventoryLockDeleted),
                    ExchangeName = nameof(IBulkInventoryLockDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                     EntityType = typeof(IBulkTotalRatingCreated),
                    QueueName = nameof(IBulkTotalRatingCreated),
                    ExchangeName = nameof(IBulkTotalRatingCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkTotalRatingDeleted),
                    QueueName = nameof(IBulkTotalRatingDeleted),
                    ExchangeName = nameof(IBulkTotalRatingDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkBreakTypeCreated),
                    QueueName = nameof(IBulkBreakTypeCreated),
                    ExchangeName = nameof(IBulkBreakTypeCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkBreakTypeDeleted),
                    QueueName = nameof(IBulkBreakTypeDeleted),
                    ExchangeName = nameof(IBulkBreakTypeDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkStandardDayPartCreated),
                    QueueName = nameof(IBulkStandardDayPartCreated),
                    ExchangeName = nameof(IBulkStandardDayPartCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkStandardDayPartDeleted),
                    QueueName = nameof(IBulkStandardDayPartDeleted),
                    ExchangeName = nameof(IBulkStandardDayPartDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkStandardDayPartGroupCreated),
                    QueueName = nameof(IBulkStandardDayPartGroupCreated),
                    ExchangeName = nameof(IBulkStandardDayPartGroupCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkStandardDayPartGroupDeleted),
                    QueueName = nameof(IBulkStandardDayPartGroupDeleted),
                    ExchangeName = nameof(IBulkStandardDayPartGroupDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkSpotBookingRuleCreated),
                    QueueName = nameof(IBulkSpotBookingRuleCreated),
                    ExchangeName = nameof(IBulkSpotBookingRuleCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkSpotBookingRuleDeleted),
                    QueueName = nameof(IBulkSpotBookingRuleDeleted),
                    ExchangeName = nameof(IBulkSpotBookingRuleDeleted)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkLengthFactorCreated),
                    QueueName = nameof(IBulkLengthFactorCreated),
                    ExchangeName = nameof(IBulkLengthFactorCreated)
                },
                new ServiceBusProducerConfigModel.Producer
                {
                    EntityType = typeof(IBulkLengthFactorDeleted),
                    QueueName = nameof(IBulkLengthFactorDeleted),
                    ExchangeName = nameof(IBulkLengthFactorDeleted)
                }
            }
        };
    }
}
