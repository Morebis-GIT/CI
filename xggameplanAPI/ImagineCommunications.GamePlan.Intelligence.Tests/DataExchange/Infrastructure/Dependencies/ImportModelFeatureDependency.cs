using BoDi;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ProgrammeCategoryHierarchy = ImagineCommunications.GamePlan.Domain.ProgrammeCategory.ProgrammeCategoryHierarchy;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Dependencies
{
    public class ImportModelFeatureDependency : IFeatureDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<ClashException>(), "ClashExceptions");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Clash>(), "Clashes");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Product>(), "Products");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Programme>(), "Programmes");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<SalesArea>(), "SalesAreas");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Metadata>(), "Metadatas");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Schedule>(), "Schedules");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Demographic>(), "Demographics");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<ProgrammeClassification>(), "ProgrammeClassifications");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Universe>(), "Universes");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Restriction>(), "Restrictions");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<ClearanceCode>(), "ClearanceCodes");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Break>(), "Breaks");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Spot>(), "Spots");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<RatingsPredictionSchedule>(), "RatingsPredictionSchedules");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<ClashException>(), "ClashExceptions");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Campaign>(), "Campaigns");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<Scenario>(), "Scenarios");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<TenantSettings>(), "TenantSettings");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<BookingPositionGroup>(), "BookingPositionGroups");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<ProgrammeCategoryHierarchy>(), "ProgrammeCategoryHierarchies");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<InventoryLockType>(), "LockTypes");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<InventoryType>(), "InventoryTypes");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<InventoryLock>(), "InventoryLocks");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<TotalRating>(), "TotalRatings");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<StandardDayPart>(), "StandardDayParts");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<StandardDayPartGroup>(), "StandardDayPartGroups");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<SpotBookingRule>(), "SpotBookingRules");
            objectContainer.RegisterFactoryAs<IImportedModel>(oc => new ImportedModel<LengthFactor>(), "LengthFactors");
        }
    }
}
