using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class SaveSmoothChanges
    {
        // Size of a batch of records to save. Overloading the database is not
        // good and causes RavenDB to hang.
        private const int DefaultBatchSize = 512;
        private const int SmallBatchHotPathSize = 32;

        private Action<string> RaiseInfo { get; }
        private IRepositoryFactory RepositoryFactory { get; }
        private ImmutableSmoothData ImmutableLookupCollection { get; }

        public SaveSmoothChanges(
            IRepositoryFactory repositoryFactory,
            ImmutableSmoothData immutableLookupCollection,
            Action<string> raiseInfo)
        {
            RaiseInfo = raiseInfo ?? throw new ArgumentNullException(nameof(raiseInfo));
            ImmutableLookupCollection = immutableLookupCollection ?? throw new ArgumentNullException(nameof(immutableLookupCollection));
            RepositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public void SaveSpotPlacements(
            DateTime processorDateTime,
            IReadOnlyCollection<Spot> spotsToSave,
            IReadOnlyDictionary<string, SpotPlacement> spotPlacementsByExternalRef,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            string batchStartEndDateForLogging
            )
        {
            if (spotsToSave is null || spotInfos is null)
            {
                return;
            }

            if (spotsToSave.Count == 0)
            {
                return;
            }

            RaiseInfo(
                "Starting to save SpotPlacements " +
                $"for {Log(spotsToSave.Count)} spots " +
                $"for {batchStartEndDateForLogging}");

            int pageNumber = 0;
            int newCount = 0;
            int updatedCount = 0;

            var batch = spotsToSave.Take(DefaultBatchSize);

            using var scope = RepositoryFactory.BeginRepositoryScope();

            do
            {
                var spotPlacementRepository = scope.CreateRepository<ISpotPlacementRepository>();

                foreach (Spot spot in batch)
                {
                    if (spotPlacementsByExternalRef.TryGetValue(
                            spot.ExternalSpotRef,
                            out SpotPlacement spotPlacement)
                        )
                    {
                        updatedCount++;

                        // Before updating ExternalBreakRef record
                        // previous value so we can reset schedule data
                        spotPlacement.ResetExternalBreakRef = spotPlacement.ExternalBreakRef;
                        spotPlacement.ModifiedTime = processorDateTime;
                        spotPlacement.ExternalBreakRef = ExternalBreakNumberOrDefault(spot);

                        spotPlacementRepository.Update(spotPlacement);
                    }
                    else
                    {
                        newCount++;

                        SpotInfo spotInfo = spotInfos[spot.Uid];

                        // A subsequent schedule data reset needs to
                        // reset SpotPlacement.ExternalBreakRef to the
                        // break that the spot was in before this run started
                        spotPlacement = new SpotPlacement()
                        {
                            ModifiedTime = processorDateTime,
                            ExternalSpotRef = spot.ExternalSpotRef,
                            ResetExternalBreakRef = spotInfo.ExternalBreakRefAtRunStart,
                            ExternalBreakRef = ExternalBreakNumberOrDefault(spot),
                        };

                        spotPlacementRepository.Add(spotPlacement);
                    }

                    try
                    {
                        spotPlacementRepository.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(
                            "Error saving Smooth spot placements for " +
                            $"spot {spotPlacement.ExternalSpotRef} for " +
                            $"batch {batchStartEndDateForLogging}",
                            exception
                            );
                    }
                }

                batch = spotsToSave
                    .Skip(DefaultBatchSize * ++pageNumber)
                    .Take(DefaultBatchSize);
            } while (batch.Any());

            RaiseInfo(
                $"Finished saving SpotPlacements for {Log(spotsToSave.Count)} spots " +
                $"(New={Log(newCount)}, Updated={Log(updatedCount)}) " +
                $"for {batchStartEndDateForLogging}"
            );

            // Local functions
            static string ExternalBreakNumberOrDefault(Spot spot) =>
                spot.IsBooked()
                    ? spot.ExternalBreakNo
                    : null;
        }

        public void SaveSmoothFailures(
            Guid runId,
            string salesAreaName,
            SmoothFailuresFactory smoothFailuresFactory,
            List<SmoothFailure> smoothFailures,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            string batchStartEndDateForLogging)
        {
            // Add Smooth failures where no attempt was made to place the spot
            // due to no break or programme data
            smoothFailures.AddRange(
                smoothFailuresFactory.CreateSmoothFailuresForNoPlaceAttempt(
                    runId,
                    salesAreaName,
                    spots,
                    spotInfos,
                    ImmutableLookupCollection.CampaignsByExternalRef,
                    ImmutableLookupCollection.ClashesByExternalRef,
                    ImmutableLookupCollection.ProductsByExternalRef));

            if (smoothFailures.Count == 0)
            {
                RaiseInfo(
                    "No smooth failures to save " +
                    $"for sales area {salesAreaName}");

                return;
            }

            RaiseInfo(
                $"Saving {Log(smoothFailures.Count)} Smooth failures " +
                $"for sales area {salesAreaName}");

            var batch = smoothFailures.Take(DefaultBatchSize);

            int pageNumber = 0;

            using var scope = RepositoryFactory.BeginRepositoryScope();
            do
            {
                var repository = scope.CreateRepository<ISmoothFailureRepository>();
                repository.AddRange(batch);

                try
                {
                    repository.SaveChanges();
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        $"Error saving Smooth failures for {batchStartEndDateForLogging}",
                        exception);
                }

                pageNumber++;
                batch = smoothFailures
                    .Skip(DefaultBatchSize * pageNumber)
                    .Take(DefaultBatchSize);
            } while (batch.Any());
        }

        public void SaveSmoothRecommendations(
            Guid runScenarioId,
            string auditMessageWithSalesAreaNameAndBatchStartEndDate,
            IReadOnlyCollection<Recommendation> recommendations)
        {
            if (recommendations is null)
            {
                RaiseInfo("No Smooth recommendations to save.");
                return;
            }

            if (recommendations.Count == 0)
            {
                RaiseInfo("No Smooth recommendations to save.");
                return;
            }

            RaiseInfo(
                $"Saving {Log(recommendations.Count)} " +
                $"Smooth recommendations {auditMessageWithSalesAreaNameAndBatchStartEndDate})"
                );

            var recommendationsWithScenarioId = new List<Recommendation>(recommendations.Count);

            foreach (var recommendation in recommendations)
            {
                var updatedRecommendation = (Recommendation)recommendation.Clone();
                updatedRecommendation.ScenarioId = runScenarioId;
                recommendationsWithScenarioId.Add(updatedRecommendation);
            }

            int pageNumber = 0;

            var batch = recommendationsWithScenarioId
                .Take(DefaultBatchSize);

            using var scope = RepositoryFactory.BeginRepositoryScope();
            while (batch.Any())
            {
                var repository = scope.CreateRepository<IRecommendationRepository>();
                repository.Insert(batch);

                try
                {
                    repository.SaveChanges();
                }
                catch (Exception exception)
                {
                    RaiseInfo(
                        "Guru meditation while saving Smooth recommendations " +
                        auditMessageWithSalesAreaNameAndBatchStartEndDate + Environment.NewLine +
                        $"{exception.Message} - {exception.StackTrace}"
                        );

                    if (exception.InnerException != null)
                    {
                        RaiseInfo(
                            "Guru meditation while saving Smooth recommendations " +
                            $"{exception.InnerException.Message} - {exception.InnerException.StackTrace}"
                            );
                    }

                    throw new Exception(
                        "Guru meditation while saving Smooth recommendations " +
                        auditMessageWithSalesAreaNameAndBatchStartEndDate,
                        exception);
                }

                pageNumber++;
                batch = recommendationsWithScenarioId
                    .Skip(DefaultBatchSize * pageNumber)
                    .Take(DefaultBatchSize);
            }

            RaiseInfo(
                $"Saved {Log(recommendations.Count)} Smooth recommendations " +
                auditMessageWithSalesAreaNameAndBatchStartEndDate);
        }

        public void SaveSmoothedSpots(IReadOnlyCollection<Spot> spotsToBatchSave)
        {
            if (spotsToBatchSave.Count == 0)
            {
                return;
            }

            using var scope = RepositoryFactory.BeginRepositoryScope();
            var spotRepository = scope.CreateRepository<ISpotRepository>();

            if (spotsToBatchSave.Count <= SmallBatchHotPathSize)
            {
                foreach (var spot in spotsToBatchSave)
                {
                    spotRepository.Update(spot);
                }
            }
            else
            {
                spotRepository.Add(spotsToBatchSave);
            }

            spotRepository.SaveChanges();
        }

        public void SaveUnplacedSpots(
            SmoothOutput smoothOutput,
            ISet<Guid> unplacedSpotIds,
            ISet<Guid> spotIdsUsed,
            ISmoothDiagnostics smoothDiagnostics)
        {
            RaiseInfo($"Updating unplaced spots for {smoothOutput.SalesAreaName}");

            if (unplacedSpotIds.Count == 0)
            {
                return;
            }

            var lazyScopeCreator = new Func<Lazy<IRepositoryScope>>(() =>
                new Lazy<IRepositoryScope>(() => RepositoryFactory.BeginRepositoryScope()));

            var lazyScope = lazyScopeCreator();
            ISpotRepository spotRepository = null;

            int countSpotsInBatch = 0;
            var unusedSpotsBatch = new List<Spot>();

            try
            {
                foreach (Guid spotIdNotUsed in unplacedSpotIds)
                {
                    if (spotIdsUsed.Contains(spotIdNotUsed))
                    {
                        continue;
                    }

                    if (spotRepository is null)
                    {
                        countSpotsInBatch = 0;
                        spotRepository = lazyScope.Value.CreateRepository<ISpotRepository>();
                    }

                    Spot spot = spotRepository.Find(spotIdNotUsed);
                    if (spot is null)
                    {
                        continue;
                    }

                    spot.ExternalBreakNo = Globals.UnplacedBreakString;

                    spotRepository.Update(spot);
                    unusedSpotsBatch.Add(spot);

                    smoothOutput.SpotsNotSet++;

                    countSpotsInBatch++;
                    if (countSpotsInBatch < DefaultBatchSize)
                    {
                        continue;
                    }

                    spotRepository.SaveChanges();
                    spotRepository = null;

                    lazyScope.Value.Dispose();
                    lazyScope = lazyScopeCreator();

                    try
                    {
                        smoothDiagnostics.LogUnplacedSmoothSpots(unusedSpotsBatch);
                    }
                    catch
                    {
                        // Ignore error writing log
                    }

                    countSpotsInBatch = 0;
                    unusedSpotsBatch.Clear();
                }

                if (countSpotsInBatch > 0 && spotRepository != null)
                {
                    spotRepository.SaveChanges();

                    try
                    {
                        smoothDiagnostics.LogUnplacedSmoothSpots(unusedSpotsBatch);
                    }
                    catch
                    {
                    }

                    unusedSpotsBatch.Clear();
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Error saving unplaced spots", exception);
            }
            finally
            {
                if (spotRepository != null)
                {
                    lazyScope.Value.Dispose();
                }
            }
        }
    }
}
