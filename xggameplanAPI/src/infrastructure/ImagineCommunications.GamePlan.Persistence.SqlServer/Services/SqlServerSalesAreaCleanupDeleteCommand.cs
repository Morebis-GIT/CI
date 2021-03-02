using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SqlServerSalesAreaCleanupDeleteCommand : Domain.Shared.SalesAreas.ISalesAreaCleanupDeleteCommand
    {
        private readonly ISqlServerTenantDbContext _dbContext;

        public SqlServerSalesAreaCleanupDeleteCommand(ISqlServerTenantDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Execute(Guid salesAreaId)
        {
            var salesArea = _dbContext.Find<SalesArea>(salesAreaId);

            if (salesArea is null)
            {
                throw new InvalidOperationException($"SalesArea with id {salesAreaId} was not found");
            }

            (Guid[] notStartedRunIds, int[] notStartedPassIds) = GetNotStartedRunPassIds();

            var breaksToDelete = GatherBreaks(salesArea);

            var clashDiffsToDelete = GatherClashDifferences(salesArea);

            (List<Restriction> restrictionsToDelete,
                List<RestrictionSalesArea> restrictionSalesAreasToDelete) = GatherRestrictions(salesArea);

            var inventoryLocksToDelete = GatherInventoryLocks(salesArea);

            var librarySalesAreaPassPrioritiesToDelete = GatherLibrarySalesAreaPassPriorities(salesArea);

            var isrSettingsToDelete = GatherIsrSettings(salesArea);

            var rsSettingsToDelete = GatherRsSettings(salesArea);

            var breakExclusionsToDelete = GatherBreakExclusions(salesArea, notStartedPassIds);

            var passSalesAreaPrioritiesToDelete = GatherPassSalesAreaPriorities(salesArea, notStartedPassIds);

            (List<PassRatingPoint> passRatingPointsToDelete,
                List<PassRatingPoint> passRatingPointsToUpdate) = GatherPassRatingPoints(salesArea, notStartedPassIds);

            var ratingsPredictionSchedulesToDelete = GatherRatingsPredictionSchedules(salesArea);

            var runSalesAreaPrioritiesToDelete = GatherRunSalesAreaPriorities(salesArea, notStartedRunIds);

            var programmesToDelete = GatherProgrammes(salesArea);

            var salesAreaDemographicsToDelete = GatherSalesAreaDemos(salesArea);

            var schedulesToDelete = GatherSchedules(salesArea);

            var universesToDelete = GatherUniverses(salesArea);

            (List<SponsorshipItem> sponsorshipItemsToDelete,
                List<SponsorshipItem> sponsorshipItemsToUpdate) = GatherSponsorshipItems(salesArea);

            (List<SpotBookingRule> spotBookingRulesToDelete,
                List<SpotBookingRuleSalesArea> spotBookingRuleSalesAreasToDelete) = GatherSpotBookingRules(salesArea);

            var spotsToDelete = GatherSpots(salesArea);

            var totalRatingsToDelete = GatherTotalRatings(salesArea);

            var transaction = _dbContext.Specific.Database.BeginTransaction();

            try
            {
                _dbContext.BulkInsertEngine.BulkDelete(breaksToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(clashDiffsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(restrictionsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(restrictionSalesAreasToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(inventoryLocksToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(librarySalesAreaPassPrioritiesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(isrSettingsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(rsSettingsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(breakExclusionsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(passSalesAreaPrioritiesToDelete);
                _dbContext.RemoveRange(passRatingPointsToDelete.ToArray());
                _dbContext.BulkInsertEngine.BulkDelete(ratingsPredictionSchedulesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(runSalesAreaPrioritiesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(programmesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(salesAreaDemographicsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(schedulesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(universesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(sponsorshipItemsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(spotBookingRulesToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(spotBookingRuleSalesAreasToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(spotsToDelete);
                _dbContext.BulkInsertEngine.BulkDelete(totalRatingsToDelete);

                _dbContext.BulkInsertEngine.BulkUpdate(passRatingPointsToUpdate);
                _dbContext.BulkInsertEngine.BulkUpdate(sponsorshipItemsToUpdate);

                _dbContext.Remove(salesArea);

                _dbContext.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private Break[] GatherBreaks(SalesArea salesArea)
        {
            var breakIds = _dbContext.Query<Break>()
                .AsNoTracking()
                .Where(x => x.SalesAreaId == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return breakIds.Select(x =>
                new Break { Id = x })
                .ToArray();
        }

        private ClashDifference[] GatherClashDifferences(SalesArea salesArea)
        {
            var clashDifferenceIds = _dbContext.Query<ClashDifference>()
                .AsNoTracking()
                .Where(x =>
                    x.SalesArea.Name == salesArea.Name
                    || x.SalesArea.Name == salesArea.ShortName)
                .Select(x => x.Id)
                .ToArray();

            return clashDifferenceIds.Select(x =>
                new ClashDifference { Id = x })
                .ToArray();
        }

        private (List<Restriction> restrictionsToDelete, List<RestrictionSalesArea> restrictionSalesAreasToDelete)
            GatherRestrictions(SalesArea salesArea)
        {
            var restrictionsToDelete = new List<Restriction>();
            var restrictionSalesAreasToDelete = new List<RestrictionSalesArea>();

            var restrictions = _dbContext.Query<Restriction>()
                .AsNoTracking()
                .Include(x => x.SalesAreas)
                .Where(x => x.SalesAreas.Any(sa =>
                    sa.SalesAreaId == salesArea.Id))
                .ToArray();

            foreach (var restriction in restrictions)
            {
                if (restriction.SalesAreas.Count == 1)
                {
                    restrictionsToDelete.Add(restriction);

                    continue;
                }

                restrictionSalesAreasToDelete.AddRange(restriction.SalesAreas
                    .Where(x =>
                        x.SalesAreaId == salesArea.Id));
            }

            return (restrictionsToDelete, restrictionSalesAreasToDelete);
        }

        private InventoryLock[] GatherInventoryLocks(SalesArea salesArea)
        {
            var inventoryLockIds = _dbContext.Query<InventoryLock>()
                .AsNoTracking()
                .Where(x =>
                    x.SalesArea.Name == salesArea.Name
                    || x.SalesArea.Name == salesArea.ShortName)
                .Select(x => x.Id)
                .ToArray();

            return inventoryLockIds.Select(x =>
                new InventoryLock { Id = x })
                .ToArray();
        }

        private SalesAreaPriority[] GatherLibrarySalesAreaPassPriorities(SalesArea salesArea)
        {
            var salesAreaPriorityIds = _dbContext.Query<SalesAreaPriority>()
                .AsNoTracking()
                .Where(x =>
                    x.SalesArea.Name == salesArea.Name
                    || x.SalesArea.ShortName == salesArea.ShortName)
                .Select(x => x.Id)
                .ToArray();

            return salesAreaPriorityIds.Select(x =>
                new SalesAreaPriority { Id = x })
                .ToArray();
        }

        private ISRSettings[] GatherIsrSettings(SalesArea salesArea)
        {
            var isrSettingsIds = _dbContext.Query<ISRSettings>()
                .AsNoTracking()
                .Where(x => x.SalesAreaId == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return isrSettingsIds.Select(x =>
                new ISRSettings { Id = x })
                .ToArray();
        }

        private RSSettings[] GatherRsSettings(SalesArea salesArea)
        {
            var rsSettingsIds = _dbContext.Query<RSSettings>()
                .AsNoTracking()
                .Where(x => x.SalesArea.Id == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return rsSettingsIds.Select(x =>
                new RSSettings { Id = x })
                .ToArray();
        }

        private PassBreakExclusion[] GatherBreakExclusions(SalesArea salesArea, int[] notStartedPassIds)
        {
            var breakExclusionIds = _dbContext.Query<PassBreakExclusion>()
                .AsNoTracking()
                .Where(x =>
                    notStartedPassIds.Contains(x.PassId)
                    && x.SalesArea.Id == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return breakExclusionIds.Select(x =>
                    new PassBreakExclusion { Id = x })
                .ToArray();
        }

        private PassSalesAreaPriority[] GatherPassSalesAreaPriorities(SalesArea salesArea, int[] notStartedPassIds)
        {
            var passSalesAreaPriorityIds =
                from passSalesAreaPriority in _dbContext.Query<PassSalesAreaPriority>().AsNoTracking()
                join passSalesAreaPriorityCollection in _dbContext.Query<PassSalesAreaPriorityCollection>()
                        .AsNoTracking()
                    on passSalesAreaPriority.PassSalesAreaPriorityCollectionId equals passSalesAreaPriorityCollection.Id
                join pass in _dbContext.Query<Pass>().AsNoTracking()
                    on passSalesAreaPriorityCollection.PassId equals pass.Id
                where (pass.IsLibraried == true
                       || notStartedPassIds.Contains(pass.Id))
                    && passSalesAreaPriority.SalesArea.Id == salesArea.Id
                select passSalesAreaPriority.Id;

            return passSalesAreaPriorityIds.Select(x =>
                new PassSalesAreaPriority { Id = x })
                .ToArray();
        }

        private (List<PassRatingPoint> passRatingPointsToDelete, List<PassRatingPoint> passRatingPointsToUpdate)
            GatherPassRatingPoints(SalesArea salesArea, int[] notStartedPassIds)
        {
            var passRatingPoints = _dbContext.Query<PassRatingPoint>()
                .AsNoTracking()
                .Include(x => x.SalesAreas)
                .Where(x => notStartedPassIds.Contains(x.PassId) &&
                    x.SalesAreas.Any(x => x.SalesAreaId.Equals(salesArea.Id)))
                .ToArray();

            var passRatingPointsToUpdate = new List<PassRatingPoint>();
            var passRatingPointsToDelete = new List<PassRatingPoint>();

            foreach (var ratingPoint in passRatingPoints)
            {
                if (ratingPoint.SalesAreas.Count == 1)
                {
                    passRatingPointsToDelete.Add(ratingPoint);

                    continue;
                }

                var salesAreaToRemove = ratingPoint.SalesAreas.FirstOrDefault(s => s.SalesAreaId == salesArea.Id);
                if (salesAreaToRemove != null)
                {
                    _ = ratingPoint.SalesAreas.Remove(salesAreaToRemove);
                    _dbContext.Remove(salesAreaToRemove);
                }

                passRatingPointsToUpdate.Add(ratingPoint);
            }

            return (passRatingPointsToDelete, passRatingPointsToUpdate);
        }

        private PredictionSchedule[] GatherRatingsPredictionSchedules(SalesArea salesArea)
        {
            var predictionScheduleIds = _dbContext.Query<PredictionSchedule>()
                .AsNoTracking()
                .Where(x => x.SalesAreaId == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return predictionScheduleIds.Select(x =>
                new PredictionSchedule { Id = x })
                .ToArray();
        }

        private RunSalesAreaPriority[] GatherRunSalesAreaPriorities(SalesArea salesArea, Guid[] notStartedRunIds)
        {
            var runSalesAreaPriorityIds =
                from runSalesAreaPriority in _dbContext.Query<RunSalesAreaPriority>().AsNoTracking()
                join run in _dbContext.Query<Run>()
                    on runSalesAreaPriority.RunId equals run.Id
                where notStartedRunIds.Contains(run.Id)
                      && runSalesAreaPriority.SalesAreaId == salesArea.Id
                select runSalesAreaPriority.Id;

            return runSalesAreaPriorityIds.Select(x =>
                new RunSalesAreaPriority { Id = x })
                .ToArray();
        }

        private Programme[] GatherProgrammes(SalesArea salesArea)
        {
            var programmeIds = _dbContext.Query<Programme>()
                .AsNoTracking()
                .Where(x => x.SalesAreaId == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return programmeIds.Select(x =>
                new Programme { Id = x })
                .ToArray();
        }

        private SalesAreaDemographic[] GatherSalesAreaDemos(SalesArea salesArea)
        {
            var salesAreaDemographicIds = _dbContext.Query<SalesAreaDemographic>()
                .AsNoTracking()
                .Where(x => x.SalesArea.Id == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return salesAreaDemographicIds.Select(x =>
                new SalesAreaDemographic { Id = x })
                .ToArray();
        }

        private Schedule[] GatherSchedules(SalesArea salesArea)
        {
            var scheduleIds = _dbContext.Query<Schedule>()
                .AsNoTracking()
                .Where(x => x.SalesAreaId == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return scheduleIds.Select(x =>
                new Schedule { Id = x })
                .ToArray();
        }

        private Universe[] GatherUniverses(SalesArea salesArea)
        {
            var universeIds = _dbContext.Query<Universe>()
                .AsNoTracking()
                .Where(x => x.SalesAreaId == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return universeIds.Select(x =>
                new Universe { Id = x })
                .ToArray();
        }

        private (List<SponsorshipItem> sponsorshipItemsToDelete, List<SponsorshipItem> sponsorshipItemsToUpdate)
            GatherSponsorshipItems(SalesArea salesArea)
        {

            var sponsorshipItems = _dbContext.Query<SponsorshipItem>()
                .AsNoTracking()
                .Include(x => x.SalesAreas)
                .Where(x =>x.SalesAreas.Any(x=> x.SalesAreaId == salesArea.Id))
                .ToArray();

            var sponsorshipItemsToDelete = new List<SponsorshipItem>();
            var sponsorshipItemsToUpdate = new List<SponsorshipItem>();

            foreach (var sponsorshipItem in sponsorshipItems)
            {
                if (sponsorshipItem.SalesAreas.Count == 1)
                {
                    sponsorshipItemsToDelete.Add(sponsorshipItem);

                    continue;
                }
                var salesAreaRef = sponsorshipItem.SalesAreas.FirstOrDefault(x => x.SalesAreaId == salesArea.Id);
                if (salesAreaRef != null)
                {
                    _ = sponsorshipItem.SalesAreas.Remove(salesAreaRef);
                    sponsorshipItemsToUpdate.Add(sponsorshipItem);
                }
            }

            return (sponsorshipItemsToDelete, sponsorshipItemsToUpdate);
        }

        private (List<SpotBookingRule> spotBookingRulesToDelete, List<SpotBookingRuleSalesArea> spotBookingRuleSalesAreasToDelete)
            GatherSpotBookingRules(SalesArea salesArea)
        {
            var spotBookingRules = _dbContext.Query<SpotBookingRule>()
                .AsNoTracking()
                .Include(x => x.SalesAreas)
                .Where(x =>
                    x.SalesAreas.Any(t => t.SalesAreaId == salesArea.Id))
                .ToArray();

            var spotBookingRulesToDelete = new List<SpotBookingRule>();
            var bookingRuleSalesAreasToDelete = new List<SpotBookingRuleSalesArea>();

            foreach (var bookingRule in spotBookingRules)
            {
                if (bookingRule.SalesAreas.Count == 1)
                {
                    spotBookingRulesToDelete.Add(bookingRule);

                    continue;
                }

                bookingRuleSalesAreasToDelete.AddRange(bookingRule.SalesAreas.Where(x => x.SalesAreaId == salesArea.Id));
            }

            return (spotBookingRulesToDelete, bookingRuleSalesAreasToDelete);
        }

        private Spot[] GatherSpots(SalesArea salesArea)
        {
            var spotIds = _dbContext.Query<Spot>()
                .AsNoTracking()
                .Where(x =>
                    x.SalesArea.Id == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return spotIds.Select(x =>
                new Spot { Id = x })
                .ToArray();
        }

        private TotalRating[] GatherTotalRatings(SalesArea salesArea)
        {
            var totalRatingIds = _dbContext.Query<TotalRating>()
                .AsNoTracking()
                .Where(x => x.SalesArea.Id == salesArea.Id)
                .Select(x => x.Id)
                .ToArray();

            return totalRatingIds.Select(x =>
                new TotalRating { Id = x })
                .ToArray();
        }

        private (Guid[] notStartedRunIds, int[] notStartedPassIds) GetNotStartedRunPassIds()
        {
            var notStartedRunIds = _dbContext.Query<Run>()
                .AsNoTracking()
                .Where(x => x.RunStatus == RunStatus.NotStarted)
                .Select(x => x.Id)
                .ToArray();

            var notStartedPassIds =
                from runScenario in _dbContext.Query<RunScenario>().AsNoTracking()
                join scenarioPassRef in _dbContext.Query<ScenarioPassReference>().AsNoTracking()
                    on runScenario.ScenarioId equals scenarioPassRef.ScenarioId
                where notStartedRunIds.Contains(runScenario.RunId)
                select scenarioPassRef.PassId;

            return (notStartedRunIds, notStartedPassIds.ToArray());
        }
    }
}
