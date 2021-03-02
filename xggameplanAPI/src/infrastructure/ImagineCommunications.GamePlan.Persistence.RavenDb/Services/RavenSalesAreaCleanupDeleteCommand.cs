using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    public class RavenSalesAreaCleanupDeleteCommand : ISalesAreaCleanupDeleteCommand
    {
        private readonly IDocumentSession _session;
        private const int BatchSize = 500_000;

        public RavenSalesAreaCleanupDeleteCommand(IDocumentSession session)
        {
            _session = session;
        }

        public void Execute(Guid salesAreaId)
        {
            var salesArea = _session.Load<SalesArea>(salesAreaId);

            if (salesArea is null)
            {
                throw new InvalidOperationException($"SalesArea with id {salesAreaId} was not found");
            }

            UpdateClashDifferences(salesArea);

            DeleteBreaks(salesArea);

            DeleteInventoryLocks(salesArea);

            DeleteIsrSettings(salesArea);

            DeleteRsSettings(salesArea);

            DeleteProgrammes(salesArea);

            DeleteSalesAreaDemographics(salesArea);

            DeleteSchedules(salesArea);

            DeleteUniverses(salesArea);

            DeleteSpots(salesArea);

            DeleteTotalRatings(salesArea);

            DeleteOrUpdateLibrarySalesAreaPassPriorities(salesArea);

            DeleteOrUpdateRestrictions(salesArea);

            DeleteOrUpdateSpotBookingRules(salesArea);

            DeleteOrUpdatePassRatingPoints(salesArea);

            DeleteRatingsPredictionSchedules(salesArea);

            UpdateClashDifferences(salesArea);

            UpdateBreakExclusions(salesArea);

            UpdateSponsorships(salesArea);

            UpdateLibrariedPassSalesAreaPriorities(salesArea);

            UpdateLibrariedScenarioPassSalesAreaPriorities(salesArea);

            UpdateNotStartedPassSalesAreaPriorities(salesArea);

            UpdateNotStartedRunsSalesAreaPriorities(salesArea);

            DeleteSalesArea(salesAreaId);
        }

        private void DeleteBreaks(SalesArea salesArea)
        {
            var breaksToDelete = GetByQuery<Break>
                (_session.Query<Break>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(breaksToDelete);
        }

        private void DeleteInventoryLocks(SalesArea salesArea)
        {
            var inventoryLocks = GetByQuery<InventoryLock>
                (_session.Query<InventoryLock>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(inventoryLocks);
        }

        private void DeleteIsrSettings(SalesArea salesArea)
        {
            var isrSettings = GetByQuery<ISRSettings>
                (_session.Query<ISRSettings>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(isrSettings);
        }

        private void DeleteRsSettings(SalesArea salesArea)
        {
            var rsSettings = GetByQuery<RSSettings>
                (_session.Query<RSSettings>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(rsSettings);
        }

        private void DeleteRatingsPredictionSchedules(SalesArea salesArea)
        {
            var predictionSchedules = GetByQuery<RatingsPredictionSchedule>
                (_session.Query<RatingsPredictionSchedule>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(predictionSchedules);
        }

        private void DeleteProgrammes(SalesArea salesArea)
        {
            var programmes = GetByQuery<Programme>
                (_session.Query<Programme>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(programmes);
        }

        private void DeleteSalesAreaDemographics(SalesArea salesArea)
        {
            var salesAreaDemographics = GetByQuery<SalesAreaDemographic>
                (_session.Query<SalesAreaDemographic>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(salesAreaDemographics);
        }

        private void DeleteSchedules(SalesArea salesArea)
        {
            var schedules = GetByQuery<Schedule>
                (_session.Query<Schedule>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(schedules);
        }

        private void DeleteUniverses(SalesArea salesArea)
        {
            var universes = GetByQuery<Universe>
                (_session.Query<Universe>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(universes);
        }

        private void DeleteSpots(SalesArea salesArea)
        {
            var spots = GetByQuery<Spot>
                (_session.Query<Spot>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(spots);
        }

        private void DeleteTotalRatings(SalesArea salesArea)
        {
            var totalRatings = GetByQuery<TotalRating>
                (_session.Query<TotalRating>()
                .Where(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName));

            BulkDelete(totalRatings);
        }

        private void DeleteOrUpdateLibrarySalesAreaPassPriorities(SalesArea salesArea)
        {
            var passPrioritiesToDelete = new List<LibrarySalesAreaPassPriority>();

            var passPriorities = GetByQuery<LibrarySalesAreaPassPriority>
                (_session.Query<LibrarySalesAreaPassPriority>()
                .Where(x => x.SalesAreaPriorities.Any(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName)));

            foreach (var passPriority in passPriorities)
            {
                if (passPriority.SalesAreaPriorities.Count == 1)
                {
                    passPrioritiesToDelete.Add(passPriority);
                }
                else
                {
                    passPriority.SalesAreaPriorities.RemoveAll(x => x.SalesArea == salesArea.Name
                    || x.SalesArea == salesArea.ShortName);
                }
            }

            _session.SaveChanges();

            BulkDelete(passPrioritiesToDelete);
        }

        private void DeleteOrUpdateRestrictions(SalesArea salesArea)
        {
            var restrictionsToDelete = new List<Restriction>();

            var restrictions = GetByQuery<Restriction>
                (_session.Query<Restriction>()
                .Where(x => x.SalesAreas.Any(sa => sa == salesArea.Name
                || sa == salesArea.ShortName)));

            foreach (var restriction in restrictions)
            {
                if (restriction.SalesAreas.Count == 1)
                {
                    restrictionsToDelete.Add(restriction);
                }
                else
                {
                    restriction.SalesAreas.RemoveAll(sa => sa == salesArea.Name
                    || sa == salesArea.ShortName);
                }
            }

            _session.SaveChanges();

            BulkDelete(restrictionsToDelete);
        }

        private void DeleteOrUpdatePassRatingPoints(SalesArea salesArea)
        {
            var notStartedPasses = GetNotStartedPasses();

            foreach (var pass in notStartedPasses)
            {
                var ratingPointsToDelete = new List<RatingPoint>();

                if (pass.RatingPoints is null)
                {
                    continue;
                }

                foreach (var ratingPoint in pass.RatingPoints)
                {
                    ratingPoint.SalesAreas = ratingPoint.SalesAreas
                        .Where(sa => sa != salesArea.Name
                        && sa != salesArea.ShortName);

                    if (!ratingPoint.SalesAreas.Any())
                    {
                        ratingPointsToDelete.Add(ratingPoint);
                    }
                }

                pass.RatingPoints = pass.RatingPoints.Where(r => !ratingPointsToDelete.Contains(r)).ToList();
            }

            _session.SaveChanges();
        }

        private void DeleteOrUpdateSpotBookingRules(SalesArea salesArea)
        {
            var spotBookingRulesToDelete = new List<SpotBookingRule>();

            var spotBookingRules = GetByQuery<SpotBookingRule>
                (_session.Query<SpotBookingRule>()
                .Where(x => x.SalesAreas.Any(sa => sa == salesArea.Name
                    || sa == salesArea.ShortName)));

            foreach (var spotBookingRule in spotBookingRules)
            {
                if (spotBookingRule.SalesAreas.Count == 1)
                {
                    spotBookingRulesToDelete.Add(spotBookingRule);
                }
                else
                {
                    spotBookingRule.SalesAreas.RemoveAll(sa => sa == salesArea.Name
                    || sa == salesArea.ShortName);
                }
            }

            _session.SaveChanges();

            BulkDelete(spotBookingRulesToDelete);
        }

        private void UpdateSponsorships(SalesArea salesArea)
        {
            var sponsorships = GetByQuery<Sponsorship>
                (_session.Query<Sponsorship>());

            var sponsoredItems = sponsorships.SelectMany(x => x.SponsoredItems).ToList();

            foreach (var sponsoredItem in sponsoredItems)
            {
                var sponsorshipItemsToDelete = new List<SponsorshipItem>();

                var sponsorshipItems = sponsoredItem.SponsorshipItems;

                foreach (var sponsorshipItem in sponsorshipItems)
                {
                    sponsorshipItem.SalesAreas = sponsorshipItem.SalesAreas
                            .Where(x => x != salesArea.Name
                            && x != salesArea.ShortName);

                    if (!sponsorshipItem.SalesAreas.Any())
                    {
                        sponsorshipItemsToDelete.Add(sponsorshipItem);
                    }
                }

                sponsoredItem.SponsorshipItems =
                    sponsoredItem.SponsorshipItems.Where(x => !sponsorshipItemsToDelete.Contains(x));
            }

            _session.SaveChanges();
        }

        private void UpdateBreakExclusions(SalesArea salesArea)
        {
            var notStartedPasses = GetNotStartedPasses();

            foreach (var pass in notStartedPasses)
            {
                if (pass.BreakExclusions is null)
                {
                    continue;
                }

                pass.BreakExclusions.RemoveAll(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName);
            }

            SaveAndClear();
        }

        private void UpdateLibrariedPassSalesAreaPriorities(SalesArea salesArea)
        {
            var librariedPasses = GetByQuery<Pass>(_session.Query<Pass>()
                .Where(p => p.IsLibraried == true && p.PassSalesAreaPriorities != null));

            foreach (var pass in librariedPasses)
            {
                pass.PassSalesAreaPriorities.SalesAreaPriorities
                    .RemoveAll(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName);
            }

            SaveAndClear();
        }

        private void UpdateLibrariedScenarioPassSalesAreaPriorities(SalesArea salesArea)
        {
            var librariedScenarioPasses = GetLibrariedScenarioPasses();

            foreach (var pass in librariedScenarioPasses)
            {
                pass.PassSalesAreaPriorities.SalesAreaPriorities
                    .RemoveAll(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName);
            }

            SaveAndClear();
        }

        private void UpdateNotStartedPassSalesAreaPriorities(SalesArea salesArea)
        {
            var notStartedPasses = GetNotStartedPasses();

            foreach (var pass in notStartedPasses)
            {
                if (pass.PassSalesAreaPriorities is null)
                {
                    continue;
                }

                pass.PassSalesAreaPriorities.SalesAreaPriorities
                    .RemoveAll(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName);
            }

            SaveAndClear();
        }

        private void UpdateNotStartedRunsSalesAreaPriorities(SalesArea salesArea)
        {
            var notStartedRuns = GetNotStartedRuns();

            foreach (var run in notStartedRuns)
            {
                if (run.SalesAreaPriorities is null)
                {
                    continue;
                }

                run.SalesAreaPriorities.RemoveAll(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName);
            }

            SaveAndClear();
        }

        private void UpdateClashDifferences(SalesArea salesArea)
        {
            var clashes = GetByQuery<Clash>(_session.Query<Clash>()
                .Where(c => c.Differences != null && c.Differences.Any()));

            foreach (var clash in clashes)
            {
                clash.Differences
                    .RemoveAll(x => x.SalesArea == salesArea.Name
                || x.SalesArea == salesArea.ShortName);
            }

            SaveAndClear();
        }

        private void DeleteSalesArea(Guid salesAreaId)
        {
            var salesArea = _session.Load<SalesArea>(salesAreaId);

            _session.Delete(salesArea);

            _session.SaveChanges();
        }

        private List<Run> GetNotStartedRuns()
        {
            return GetByQuery<Run>(_session.Query<Run>()
                .Where(x => x.RunStatus == RunStatus.NotStarted));
        }

        private List<Pass> GetNotStartedPasses()
        {
            var notStartedRuns = GetNotStartedRuns();

            var runScenarios = notStartedRuns.SelectMany(s => s.Scenarios);

            var runScenarioIds = runScenarios.Select(s => s.Id);

            var scenarios = GetByQuery<Scenario>(_session.Query<Scenario>()
                .Where(x => x.Id.In<Guid>(runScenarioIds)));

            var scenarioPasses = scenarios.SelectMany(s => s.Passes);

            var notStartedPassIds = scenarioPasses.Select(p => p.Id);

            var notStartedPasses = GetByQuery<Pass>(_session.Query<Pass>()
                .Where(x => x.Id.In<int>(notStartedPassIds)));

            return notStartedPasses;
        }

        private List<Pass> GetLibrariedScenarioPasses()
        {
            var librariedScenarios = GetByQuery<Scenario>(_session.Query<Scenario>()
                .Where(x => x.IsLibraried == true));

            var librariedScenarioPasses = librariedScenarios.SelectMany(s => s.Passes);

            var passIds = librariedScenarioPasses.Select(p => p.Id);

            var passes = GetByQuery<Pass>(_session.Query<Pass>()
                .Where(x => x.Id.In<int>(passIds)));

            return passes;
        }

        private void BulkDelete<T>(IEnumerable<T> entities) where T : class
        {
            var recordNumber = 0;

            foreach (var entity in entities)
            {
                _session.Delete<T>(entity);
                recordNumber++;

                if (recordNumber == BatchSize)
                {
                    SaveAndClear();

                    recordNumber = 0;
                }
            }

            SaveAndClear();
        }

        private List<T> GetByQuery<T>(IRavenQueryable<T> query) where T : class
        {
            var results = new List<T>();
            var start = 0;

            while (true)
            {
                var current = query.Take(1024).Skip(start).ToList();

                if (current.Count == 0)
                {
                    break;
                }

                start += current.Count;
                results.AddRange(current);
            }

            return results;
        }

        private void SaveAndClear()
        {
            _session.SaveChanges();

            _session.Advanced.Clear();
        }
    }
}
