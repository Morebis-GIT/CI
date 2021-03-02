using BoDi;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.BookingPositionGroup;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Campaign;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.ClashException;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.InventoryStatus;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Programme;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.ProgrammeCategory;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.ProgrammeClassification;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Restriction;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.SalesArea;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Spot;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Universe;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Dependencies
{
    public class ResultCheckerScenarioDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterTypeAs<ClashResultChecker, IResultCheckerService>("Clashes");
            objectContainer.RegisterTypeAs<ProductResultChecker, IResultCheckerService>("Products");
            objectContainer.RegisterTypeAs<HolidayResultChecker, IResultCheckerService>("Holidays");
            objectContainer.RegisterTypeAs<DemographicResultChecker, IResultCheckerService>("Demographics");
            objectContainer.RegisterTypeAs<ProgrammeResultChecker, IResultCheckerService>("Programmes");
            objectContainer.RegisterTypeAs<ProgrammeScheduleResultChecker, IResultCheckerService>("Schedules");
            objectContainer.RegisterTypeAs<ProgrammeClassificationResultChecker, IResultCheckerService>("ProgrammeClassifications");
            objectContainer.RegisterTypeAs<UniverseResultChecker, IResultCheckerService>("Universes");
            objectContainer.RegisterTypeAs<RestrictionResultChecker, IResultCheckerService>("Restrictions");
            objectContainer.RegisterTypeAs<SpotResultChecker, IResultCheckerService>("Spots");
            objectContainer.RegisterTypeAs<BreakResultChecker, IResultCheckerService>("Breaks");
            objectContainer.RegisterTypeAs<RatingsPredictionScheduleResultChecker, IResultCheckerService>("RatingsPredictionSchedules");
            objectContainer.RegisterTypeAs<SalesAreaResultChecker, IResultCheckerService>("SalesAreas");
            objectContainer.RegisterTypeAs<ClashExceptionResultChecker, IResultCheckerService>("ClashExceptions");
            objectContainer.RegisterTypeAs<CampaignResultChecker, IResultCheckerService>("Campaigns");
            objectContainer.RegisterTypeAs<ScenarioResultChecker, IResultCheckerService>("Scenarios");
            objectContainer.RegisterTypeAs<BookingPositionGroupResultChecker, IResultCheckerService>("BookingPositionGroups");
            objectContainer.RegisterTypeAs<ProgrammeCategoryResultChecker, IResultCheckerService>("ProgrammeCategoryHierarchies");
            objectContainer.RegisterTypeAs<LockTypeResultChecker, IResultCheckerService>("LockTypes");
            objectContainer.RegisterTypeAs<InventoryTypeResultChecker, IResultCheckerService>("InventoryTypes");
            objectContainer.RegisterTypeAs<InventoryLockResultChecker, IResultCheckerService>("InventoryLocks");
            objectContainer.RegisterTypeAs<TotalRatingResultChecker, IResultCheckerService>("TotalRatings");
            objectContainer.RegisterTypeAs<MetadataResultChecker, IResultCheckerService>("Metadatas");
            objectContainer.RegisterTypeAs<StandardDayPartResultChecker, IResultCheckerService>("StandardDayParts");
            objectContainer.RegisterTypeAs<StandardDayPartGroupResultChecker, IResultCheckerService>("StandardDayPartGroups");
            objectContainer.RegisterTypeAs<SpotBookingRuleResultChecker, IResultCheckerService>("SpotBookingRules");
            objectContainer.RegisterTypeAs<LengthFactorResultChecker, IResultCheckerService>("LengthFactors");
        }
    }
}
