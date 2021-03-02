using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace xggameplan.RunManagement
{
    /// <summary>
    /// Reset for schedule data (i.e. Breaks, campaigns, schedules, spots etc)
    /// </summary>
    internal class ScheduleReset
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ScheduleReset(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Resets schedule data. i.e. Breaks, Spots, SpotPlacements, Schedules etc for the sales areas and date range.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="salesAreas"></param>
        /// <param name="resetBreakEfficiency"></param>
        /// <param name="resetSpotBreakPlacing"></param>
        /// <param name="resetSpotPlacement"></param>
        public void ResetScheduleData(
            List<string> salesAreas,
            DateTime fromDate,
            DateTime toDate,
            bool resetBreakEfficiency,
            bool resetSpotBreakPlacing,
            bool resetSpotPlacement
            )
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();

                // Get sales areas for the run
                var salesAreaList = salesAreaRepository.GetAll().Where(sa => salesAreas.Contains(sa.Name));
                SemaphoreSlim semaphore = new SemaphoreSlim(10);   // Limit threads
                Mutex sharedResourceMutex = new Mutex();
                var tasks = new List<Task>();
                foreach (var salesArea in salesAreaList)
                {
                    semaphore.Wait();       // Wait for free thread
                    var task = Task.Factory.StartNew(() =>
                    {
                        ResetScheduleData(salesArea, fromDate, toDate, sharedResourceMutex, resetBreakEfficiency, resetSpotBreakPlacing, resetSpotPlacement);
                    }).ContinueWith(myTask =>
                    {
                        semaphore.Release();        // Release thread
                        if (myTask.Exception != null)
                        {
                            throw new Exception(string.Format("Error resetting schedule data for {0}", salesArea.Name), myTask.Exception);
                        }
                    });
                    tasks.Add(task);
                }

                // Wait for all tasks to complete
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Resets schedule data. i.e. Break availability, Break efficiency, Spot breaks.
        /// </summary>
        /// <param name="salesArea"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sharedResourceMutex"></param>
        /// <param name="resetBreakAvailability"></param>
        /// <param name="resetBreakEfficiency"></param>
        /// <param name="resetSpotBreakPlacing"></param>
        /// <param name="resetSpotPlacement"></param>
        private void ResetScheduleData(
            SalesArea salesArea,
            DateTime fromDateTime,
            DateTime toDateTime,
            Mutex sharedResourceMutex,
            bool resetBreakEfficiency,
            bool resetSpotBreakPlacing,
            bool resetSpotPlacementBreaks
            )
        {
            foreach (DateTime[] dateRange in DateHelper.GetDateRanges(fromDateTime, toDateTime, 14))
            {
                ResetSpots(salesArea, dateRange[0], dateRange[1], resetSpotBreakPlacing, resetSpotPlacementBreaks, sharedResourceMutex);
                ResetBreaks(salesArea, dateRange[0], dateRange[1], resetBreakEfficiency, sharedResourceMutex);
                ResetSchedules(salesArea, dateRange[0], dateRange[1], resetBreakEfficiency, sharedResourceMutex);
            }
        }

        /// <summary>
        /// Resets spots (E.g. Break placing)
        /// </summary>
        /// <param name="salesArea"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="resetSpotBreakPlacing"></param>
        /// <param name="resetSpotPlacement"></param>
        /// <param name="sharedResourceMutex"></param>
        private void ResetSpots(SalesArea salesArea, DateTime fromDateTime, DateTime toDateTime, bool resetSpotBreakPlacing, bool resetSpotPlacementBreaks, Mutex sharedResourceMutex)
        {
            if (!resetSpotBreakPlacing && !resetSpotPlacementBreaks)
            {
                return;
            }

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                // Create repositories
                sharedResourceMutex.WaitOne();
                var spotRepository = scope.CreateRepository<ISpotRepository>();
                sharedResourceMutex.ReleaseMutex();

                // Get spots to reset
                var spots = spotRepository.Search(fromDateTime, toDateTime, salesArea.Name).ToList();
                if (resetSpotBreakPlacing)
                {
                    foreach (var spot in spots)
                    {
                        if (resetSpotBreakPlacing)
                        {
                            //spot.ExternalBreakNo = spot.ResetExternalBreakNo;
                            //spot.ActualPositioninBreak = spot.ResetActualPositioninBreak;
                            //spotRepository.Add(spot);
                        }
                    }
                }
                spotRepository.SaveChanges();

                // Reset spot placements
                if (resetSpotPlacementBreaks)
                {
                    using (var scope2 = scope.BeginRepositoryScope())
                    {
                        var spotPlacementRepository = scope2.CreateRepository<ISpotPlacementRepository>();
                        var spotPlacements =
                            spotPlacementRepository.GetByExternalSpotRefs(spots.Select(s => s.ExternalSpotRef)
                                .Distinct());
                        foreach (var spotPlacement in spotPlacements)
                        {
                            spotPlacement.ExternalBreakRef = spotPlacement.ResetExternalBreakRef;
                        }

                        spotPlacementRepository.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Resets breaks (E.g. Availability, efficiency)
        /// </summary>
        /// <param name="breaks"></param>
        /// <param name="spots"></param>
        /// <param name="breakRepository"></param>
        private void ResetBreaks(SalesArea salesArea, DateTime fromDateTime, DateTime toDateTime,
            bool resetBreakEfficiency, Mutex sharedResourceMutex)
        {
            if (!resetBreakEfficiency)
            {
                return;
            }

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                // Get breaks & schedules to reset
                // Create repositories
                sharedResourceMutex.WaitOne();
                var breakRepository = scope.CreateRepository<IBreakRepository>();
                sharedResourceMutex.ReleaseMutex();

                var breaks = breakRepository.Search(fromDateTime, toDateTime, salesArea.Name).ToList();
                foreach (var @break in breaks)
                {
                    if (resetBreakEfficiency)
                    {
                        @break.BreakEfficiencyList = null;
                    }
                    breakRepository.Add(@break);
                }
                breakRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Resets breaks (E.g. Availability, efficiency)
        /// </summary>
        /// <param name="breaks"></param>
        /// <param name="spots"></param>
        /// <param name="breakRepository"></param>
        private void ResetSchedules(
            SalesArea salesArea,
            DateTime fromDateTime,
            DateTime toDateTime,
            bool resetBreakEfficiency,
            Mutex sharedResourceMutex
            )
        {
            if (!resetBreakEfficiency)
            {
                return;
            }

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                // Get repository
                sharedResourceMutex.WaitOne();
                var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
                sharedResourceMutex.ReleaseMutex();

                var schedules = scheduleRepository.GetSchedule(new List<string>() { salesArea.Name }, fromDateTime, toDateTime);
                foreach (var schedule in schedules)
                {
                    if (schedule.Breaks != null)
                    {
                        foreach (var @break in schedule.Breaks)
                        {
                            if (resetBreakEfficiency)
                            {
                                @break.BreakEfficiencyList = null;
                            }

                        }
                        scheduleRepository.Add(schedule);
                    }
                }
                scheduleRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes spot placement data, data older than created time limit
        /// </summary>
        /// <param name="modifiedTime"></param>
        public void DeleteSpotPlacementData(DateTime modifiedTime)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var spotPlacementRepository = scope.CreateRepository<ISpotPlacementRepository>();
                spotPlacementRepository.DeleteBefore(modifiedTime);
                spotPlacementRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Updates spot placement data for all spots for the sales areas and date range.
        ///
        /// This is not part of ResetScheduleData because that method resets data to an initial state. This method captures the current placement
        /// of all spots which is different.
        /// </summary>
        /// <param name="salesAreas"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        public void UpdateSpotPlacementData(IReadOnlyList<SalesArea> salesAreas, DateTime fromDate, DateTime toDate)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var repositories = scope.CreateRepositories(
                    typeof(ISalesAreaRepository),
                    typeof(ISpotRepository)
                );
                var salesAreaRepository = repositories.Get<ISalesAreaRepository>();
                var spotRepository = repositories.Get<ISpotRepository>();

                DateTime modifiedTime = DateTime.UtcNow;
                SemaphoreSlim semaphore = new SemaphoreSlim(10);   // Limit threads
                Mutex sharedResourceMutex = new Mutex();
                var tasks = new List<Task>();
                foreach (var salesArea in salesAreas)
                {
                    semaphore.Wait();       // Wait for free thread
                    var task = Task.Factory.StartNew(() =>
                    {
                        int countSpotsDoneForSalesArea = UpdateSpotPlacementData(salesArea, sharedResourceMutex, fromDate, toDate, modifiedTime);
                    }).ContinueWith(myTask =>
                    {
                        semaphore.Release();        // Release thread
                        if (myTask.Exception != null)
                        {
                            throw new Exception(string.Format("Error creating spot placement data for {0}", salesArea.Name), myTask.Exception);
                        }
                    });
                    tasks.Add(task);
                }

                // Wait for all tasks to complete
                Task.WaitAll(tasks.ToArray());
            }
        }

        private int UpdateSpotPlacementData(SalesArea salesArea, Mutex sharedResourceMutex, DateTime fromDate, DateTime toDate, DateTime modifiedTime)
        {
            int countSpotsDone = 0;

            // Process spots in date range batches
            foreach (DateTime[] dateRange in DateHelper.GetDateRanges(fromDate, toDate, 7))
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    sharedResourceMutex.WaitOne();
                    var spotRepository = scope.CreateRepository<ISpotRepository>();
                    sharedResourceMutex.ReleaseMutex();

                    // Get spots for date range
                    var spots = spotRepository.Search(dateRange[0], dateRange[1], salesArea.Name).OrderBy(s => s.ExternalSpotRef);
                    HashSet<string> spotsDone = new HashSet<string>();

                    // Create spot placements, saved in batches
                    List<Spot> spotBatch = new List<Spot>();
                    foreach (var spot in spots)
                    {
                        countSpotsDone++;
                        spotBatch.Add(spot);

                        if ((spotBatch.Count >= 1000) || (spot == spots.Last()))
                        {
                            using (var scope2 = scope.BeginRepositoryScope())
                            {
                                sharedResourceMutex.WaitOne();
                                var spotPlacementRepository = scope2.CreateRepository<ISpotPlacementRepository>();
                                sharedResourceMutex.ReleaseMutex();

                                // Get spot placements
                                Dictionary<string, SpotPlacement> spotPlacementedByExternalSpotRef = SpotPlacement.IndexListByExternalSpotRef(spotPlacementRepository.GetByExternalSpotRefs(spotBatch.Select(s => s.ExternalSpotRef)));

                                // Create/update spot placements for each spot
                                List<SpotPlacement> spotPlacementBatch = new List<SpotPlacement>();
                                foreach (var spotInBatch in spotBatch)
                                {
                                    SpotPlacement spotPlacement = spotPlacementedByExternalSpotRef.ContainsKey(spotInBatch.ExternalSpotRef) ? spotPlacementedByExternalSpotRef[spotInBatch.ExternalSpotRef] : null;
                                    if (spotPlacement == null)      // Newly encountered spot
                                    {
                                        spotPlacement = new SpotPlacement()
                                        {
                                            ModifiedTime = modifiedTime,
                                            ExternalSpotRef = spotInBatch.ExternalSpotRef,
                                            ExternalBreakRef = spot.ExternalBreakNo
                                        };
                                        spotPlacementBatch.Add(spotPlacement);      // spotPlacementRepository.Insert(new List<SpotPlacement>() { spotPlacement });
                                    }
                                    else     // Existing spot
                                    {
                                        spotPlacement.ModifiedTime = modifiedTime;
                                        spotPlacement.ExternalBreakRef = spot.ExternalBreakNo;
                                        spotPlacementRepository.Update(spotPlacement);
                                    }
                                }
                                if (spotPlacementBatch.Any())
                                {
                                    spotPlacementRepository.Insert(spotPlacementBatch);
                                }
                                spotPlacementRepository.SaveChanges();

                                // Clear batch
                                spotBatch.Clear();
                            }
                        }

                        if (countSpotsDone % 10000 == 0)
                        {
                            System.Diagnostics.Debug.Write(string.Format("Created {0} SpotPlacement documents for {1}", countSpotsDone, salesArea.Name));
                        }
                    }
                }
            }
            return countSpotsDone;
        }

        /// <summary>
        /// Updates break availability for Optimiser
        /// </summary>
        /// <param name="salesAreas"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        public void UpdateOptimiserBreakAvailablity(List<SalesArea> salesAreas, DateTime fromDate, DateTime toDate)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();

                SemaphoreSlim semaphore = new SemaphoreSlim(10);   // Limit threads
                Mutex sharedResourceMutex = new Mutex();
                var tasks = new List<Task>();
                foreach (var salesArea in salesAreas)
                {
                    semaphore.Wait();       // Wait for free thread
                    var task = Task.Factory.StartNew(() =>
                    {
                        UpdateOptimiserBreakAvailability(salesArea, fromDate, toDate);
                    }).ContinueWith(myTask =>
                    {
                        semaphore.Release();        // Release thread
                        if (myTask.Exception != null)
                        {
                            throw new Exception(string.Format("Error creating spot placement data for {0}", salesArea.Name), myTask.Exception);
                        }
                    });
                    tasks.Add(task);
                }

                // Wait for all tasks to complete
                Task.WaitAll(tasks.ToArray());
            }
        }

        private void UpdateOptimiserBreakAvailability(SalesArea salesArea, DateTime fromDate, DateTime toDate)
        {
            DateTime currentScheduleDate = fromDate.AddDays(-1);
            while (currentScheduleDate < toDate)
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    currentScheduleDate = currentScheduleDate.AddDays(1);

                    // Update Schedule documents
                    var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
                    var schedule = scheduleRepository.GetSchedule(salesArea.Name, currentScheduleDate.Date);
                    if (schedule != null && schedule.Breaks != null)
                    {
                        foreach (var @break in schedule.Breaks)
                        {
                            @break.OptimizerAvail = @break.Avail;
                        }
                        scheduleRepository.Add(schedule);
                        scheduleRepository.SaveChanges();
                    }

                    // Update Break documents
                    var breakRepository = scope.CreateRepository<IBreakRepository>();
                    var breaks = breakRepository.Search(currentScheduleDate.Date, currentScheduleDate.Date, salesArea.Name);
                    if (breaks.Any())
                    {
                        foreach (var @break in breaks)
                        {
                            @break.OptimizerAvail = @break.Avail;
                            breakRepository.Add(@break);
                        }
                        breakRepository.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Creates SpotPlacement instances for all existing spots
        /// </summary>
        public void CreateSpotPlacementData(List<SalesArea> salesAreas, DateTime fromDate, DateTime toDate)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var repositories = scope.CreateRepositories(typeof(ISalesAreaRepository), typeof(ISpotRepository));
                var salesAreaRepository = repositories.Get<ISalesAreaRepository>();
                var spotRepository = repositories.Get<ISpotRepository>();

                DateTime modifiedTime = DateTime.UtcNow;
                SemaphoreSlim semaphore = new SemaphoreSlim(10);   // Limit threads
                Mutex sharedResourceMutex = new Mutex();
                var tasks = new List<Task>();
                foreach (var salesArea in salesAreas)
                {
                    semaphore.Wait();       // Wait for free thread
                    var task = Task.Factory.StartNew(() =>
                    {
                        int countSpotsDoneForSalesArea = CreateSpotPlacementData(salesArea, sharedResourceMutex, fromDate, toDate, modifiedTime);
                    }).ContinueWith(myTask =>
                    {
                        semaphore.Release();        // Release thread
                        if (myTask.Exception != null)
                        {
                            throw new Exception(string.Format("Error creating spot placement data for {0}", salesArea.Name), myTask.Exception);
                        }
                    });
                    tasks.Add(task);
                }

                // Wait for all tasks to complete
                Task.WaitAll(tasks.ToArray());
            }
        }

        private int CreateSpotPlacementData(SalesArea salesArea, Mutex sharedResourceMutex, DateTime fromDate, DateTime toDate, DateTime modifiedTime)
        {
            int countSpotsDone = 0;

            // Process spots in date range batches
            foreach (DateTime[] dateRange in DateHelper.GetDateRanges(fromDate, toDate, 7))
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    sharedResourceMutex.WaitOne();
                    var spotRepository = scope.CreateRepository<ISpotRepository>();
                    sharedResourceMutex.ReleaseMutex();

                    // Get spots for date range
                    var spots = spotRepository.Search(dateRange[0], dateRange[1], salesArea.Name).OrderBy(s => s.ExternalSpotRef);
                    HashSet<string> spotsDone = new HashSet<string>();

                    // Create spot placements, saved in batches
                    List<SpotPlacement> spotPlacementBatch = new List<SpotPlacement>();
                    foreach (var spot in spots)
                    {
                        countSpotsDone++;
                        SpotPlacement spotPlacement = new SpotPlacement()
                        {
                            ModifiedTime = modifiedTime,
                            ExternalSpotRef = spot.ExternalSpotRef,
                            ExternalBreakRef = spot.ExternalBreakNo
                        };
                        if (!spotsDone.Contains(spotPlacement.ExternalSpotRef))  // Handle duplicates
                        {
                            spotsDone.Add(spotPlacement.ExternalSpotRef);
                            spotPlacementBatch.Add(spotPlacement);
                        }

                        // Save batch
                        if ((spotPlacementBatch.Count >= 10000) || ((spot == spots.Last() && spotPlacementBatch.Count > 0)))
                        {
                            using (var scope2 = scope.BeginRepositoryScope())
                            {
                                sharedResourceMutex.WaitOne();
                                var spotPlacementRepository = scope2.CreateRepository<ISpotPlacementRepository>();
                                sharedResourceMutex.ReleaseMutex();
                                spotPlacementRepository.Insert(spotPlacementBatch);
                                spotPlacementRepository.SaveChanges();
                                spotPlacementBatch.Clear();
                            }
                        }

                        if (countSpotsDone % 10000 == 0)
                        {
                            System.Diagnostics.Debug.Write(string.Format("Created {0} SpotPlacement documents for {1}", countSpotsDone, salesArea.Name));
                        }
                    }
                }
            }
            return countSpotsDone;
        }
    }
}
