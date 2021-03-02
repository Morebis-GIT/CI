using BoDi;
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
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.EventScenarioServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Dependencies
{
    public class EventServiceScenarioDependency : IScenarioDependency
    {
        public EventServiceScenarioDependency() { }

        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkClashCreatedOrUpdated, BulkClashCreatedOrUpdated, IClashCreatedOrUpdated, ClashCreatedOrUpdated>, IEventScenarioService>("BulkClashCreatedOrUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkClashDeleted, BulkClashDeleted, IClashDeleted, ClashDeleted>, IEventScenarioService>("BulkClashDeleted");
            objectContainer.RegisterTypeAs<EventScenarioService<IClashUpdated, ClashUpdated>, IEventScenarioService>("ClashUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IClashTruncated, ClashTruncated>, IEventScenarioService>("ClashTruncated");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkBreakCreated, BulkBreakCreated, IBreakCreated, BreakCreated>, IEventScenarioService>("BulkBreakCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkBreaksDeleted, BulkBreaksDeleted, IBreakDeleted, BreakDeleted>, IEventScenarioService>("BulkBreaksDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProductCreatedOrUpdated, BulkProductCreatedOrUpdated, IProductCreatedOrUpdated, ProductCreatedOrUpdated>, IEventScenarioService>("BulkProductCreatedOrUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProductDeleted, BulkProductDeleted, IProductDeleted, ProductDeleted>, IEventScenarioService>("BulkProductDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkHolidayCreated, BulkHolidayCreated, IHolidayCreated, HolidayCreated>, IEventScenarioService>("BulkHolidayCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkHolidayDeleted, BulkHolidayDeleted, IHolidayDeleted, HolidayDeleted>, IEventScenarioService>("BulkHolidayDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IDemographicUpdated, DemographicUpdated>, IEventScenarioService>("DemographicUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkDemographicDeleted, BulkDemographicDeleted, IDemographicDeleted, DemographicDeleted>, IEventScenarioService>("BulkDemographicDeleted");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkDemographicCreatedOrUpdated, BulkDemographicCreatedOrUpdated, IDemographicCreatedOrUpdated, DemographicCreatedOrUpdated>, IEventScenarioService>("BulkDemographicCreatedOrUpdated");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProgrammeCreated, BulkProgrammeCreated, IProgrammeCreated, ProgrammeCreated>, IEventScenarioService>("BulkProgrammeCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProgrammeUpdated, BulkProgrammeUpdated, IProgrammeUpdated, ProgrammeUpdated>, IEventScenarioService>("BulkProgrammeUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProgrammeDeleted, BulkProgrammeDeleted, IProgrammesDeleted, ProgrammesDeleted>, IEventScenarioService>("BulkProgrammeDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProgrammeClassificationCreated, BulkProgrammeClassificationCreated, IProgrammeClassificationCreated, ProgrammeClassificationCreated>, IEventScenarioService>("BulkProgrammeClassificationCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IDeleteAllProgrammeClassification, DeleteAllProgrammeClassification>, IEventScenarioService>("DeleteAllProgrammeClassification");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkUniverseCreated, BulkUniverseCreated, IUniverseCreated, UniverseCreated>, IEventScenarioService>("BulkUniverseCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkUniverseDeleted, BulkUniverseDeleted, IUniverseDeleted, UniverseDeleted>, IEventScenarioService>("BulkUniverseDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkRestrictionCreatedOrUpdated, BulkRestrictionCreatedOrUpdated, IRestrictionCreatedOrUpdated, RestrictionCreatedOrUpdated>, IEventScenarioService>("BulkRestrictionCreatedOrUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkRestrictionDeleted, BulkRestrictionDeleted, IRestrictionDeleted, RestrictionDeleted>, IEventScenarioService>("BulkRestrictionDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkSpotCreatedOrUpdated, BulkSpotCreatedOrUpdated, ISpotCreatedOrUpdated, SpotCreatedOrUpdated>, IEventScenarioService>("BulkSpotCreatedOrUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkSpotDeleted, BulkSpotDeleted, ISpotDeleted, SpotDeleted>, IEventScenarioService>("BulkSpotDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkRatingsPredictionSchedulesCreated, BulkRatingsPredictionScheduleCreated, IRatingsPredictionScheduleCreated, RatingsPredictionScheduleCreated>, IEventScenarioService>("BulkRatingsPredictionScheduleCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkRatingsPredictionSchedulesDeleted, BulkRatingsPredictionSchedulesDeleted, IRatingsPredictionSchedulesDeleted, RatingsPredictionSchedulesDeleted>, IEventScenarioService>("BulkRatingsPredictionSchedulesDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkSalesAreaCreatedOrUpdated, BulkSalesAreaCreatedOrUpdated, ISalesAreaCreatedOrUpdated, SalesAreaCreatedOrUpdated>, IEventScenarioService>("BulkSalesAreaCreatedOrUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkSalesAreaDeleted, BulkSalesAreaDeleted, ISalesAreaDeleted, SalesAreaDeleted>, IEventScenarioService>("BulkSalesAreaDeleted");
            objectContainer.RegisterTypeAs<EventScenarioService<ISalesAreaUpdated, SalesAreaUpdated>, IEventScenarioService>("SalesAreaUpdated");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkClashExceptionCreated, BulkClashExceptionCreated, IClashExceptionCreated, ClashExceptionCreated>, IEventScenarioService>("BulkClashExceptionCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkClashExceptionDeleted, BulkClashExceptionDeleted, IClashExceptionDeleted, ClashExceptionDeleted>, IEventScenarioService>("BulkClashExceptionDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkCampaignCreatedOrUpdated, BulkCampaignCreatedOrUpdated, ICampaignCreatedOrUpdated, CampaignCreatedOrUpdated>, IEventScenarioService>("BulkCampaignCreatedOrUpdated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkCampaignDeleted, BulkCampaignDeleted, ICampaignDeleted, CampaignDeleted>, IEventScenarioService>("BulkCampaignDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkBookingPositionGroupCreated, BulkBookingPositionGroupCreated, IBookingPositionGroupCreated, BookingPositionGroupCreated>, IEventScenarioService>("BulkBookingPositionGroupCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkBookingPositionGroupDeleted, BulkBookingPositionGroupDeleted, IBookingPositionGroupDeleted, BookingPositionGroupDeleted>, IEventScenarioService>("BulkBookingPositionGroupDeleted");
            objectContainer.RegisterTypeAs<EventScenarioService<IBookingPositionGroupTruncated, BookingPositionGroupTruncated>, IEventScenarioService>("BookingPositionGroupTruncated");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProgrammeCategoryCreated, BulkProgrammeCategoryCreated, IProgrammeCategoryCreated, ProgrammeCategoryCreated>, IEventScenarioService>("BulkProgrammeCategoryCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkProgrammeCategoryDeleted, BulkProgrammeCategoryDeleted>, IEventScenarioService>("BulkProgrammeCategoryDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkLockTypeCreated, BulkLockTypeCreated, ILockTypeCreated, LockTypeCreated>, IEventScenarioService>("BulkLockTypeCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkLockTypeDeleted, BulkLockTypeDeleted>, IEventScenarioService>("BulkLockTypeDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkInventoryTypeCreated, BulkInventoryTypeCreated, IInventoryTypeCreated, InventoryTypeCreated>, IEventScenarioService>("BulkInventoryTypeCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkInventoryTypeDeleted, BulkInventoryTypeDeleted>, IEventScenarioService>("BulkInventoryTypeDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkInventoryLockCreated, BulkInventoryLockCreated, IInventoryLockCreated, InventoryLockCreated>, IEventScenarioService>("BulkInventoryLockCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkInventoryLockDeleted, BulkInventoryLockDeleted, IInventoryLockDeleted, InventoryLockDeleted>, IEventScenarioService>("BulkInventoryLockDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkTotalRatingCreated, BulkTotalRatingCreated, ITotalRatingCreated, TotalRatingCreated>, IEventScenarioService>("BulkTotalRatingCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkTotalRatingDeleted, BulkTotalRatingDeleted, ITotalRatingDeleted, TotalRatingDeleted>, IEventScenarioService>("BulkTotalRatingDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkBreakTypeCreated, BulkBreakTypeCreated, IBreakTypeCreated, BreakTypeCreated>, IEventScenarioService>("BulkBreakTypeCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkBreakTypeDeleted, BulkBreakTypeDeleted>, IEventScenarioService>("BulkBreakTypeDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkStandardDayPartCreated, BulkStandardDayPartCreated, IStandardDayPartCreated, StandardDayPartCreated>, IEventScenarioService>("BulkStandardDayPartCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkStandardDayPartDeleted, BulkStandardDayPartDeleted>, IEventScenarioService>("BulkStandardDayPartDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkStandardDayPartGroupCreated, BulkStandardDayPartGroupCreated, IStandardDayPartGroupCreated, StandardDayPartGroupCreated>, IEventScenarioService>("BulkStandardDayPartGroupCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkStandardDayPartGroupDeleted, BulkStandardDayPartGroupDeleted>, IEventScenarioService>("BulkStandardDayPartGroupDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkSpotBookingRuleCreated, BulkSpotBookingRuleCreated, ISpotBookingRuleCreated, SpotBookingRuleCreated>, IEventScenarioService>("BulkSpotBookingRuleCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkSpotBookingRuleDeleted, BulkSpotBookingRuleDeleted>, IEventScenarioService>("BulkSpotBookingRuleDeleted");

            objectContainer.RegisterTypeAs<EventScenarioService<IBulkLengthFactorCreated, BulkLengthFactorCreated, ILengthFactorCreated, LengthFactorCreated>, IEventScenarioService>("BulkLengthFactorCreated");
            objectContainer.RegisterTypeAs<EventScenarioService<IBulkLengthFactorDeleted, BulkLengthFactorDeleted>, IEventScenarioService>("BulkLengthFactorDeleted");
        }
    }
}
