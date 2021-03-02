using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class BreakEfficiencyOutputFileProcessor : IOutputFileProcessor<ProcessBreakEfficiencyOutput>
    {
        private readonly IEfficiencySettingsRepository _efficiencySettingsRepository;
        private readonly IOutputDataSnapshot _dataSnapshot;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;

        public BreakEfficiencyOutputFileProcessor(IEfficiencySettingsRepository efficiencySettingsRepository, IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository, IOutputDataSnapshot dataSnapshot)
        {
            _efficiencySettingsRepository = efficiencySettingsRepository;
            _dataSnapshot = dataSnapshot;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public string FileName { get; } = OutputFileNames.BreakEfficiency;

        public ProcessBreakEfficiencyOutput ProcessFile(Guid scenarioId, string folder)
        {
            var output = new ProcessBreakEfficiencyOutput();
            var run = _dataSnapshot.Run.Value;
            var efficiencySettings = _efficiencySettingsRepository.GetDefault();

            if (!run.Manual || run.Manual && efficiencySettings.PersistEfficiency == PersistEfficiency.AllRun)
            {
                if (scenarioId == run.Scenarios.OrderBy(c => c.Id).First().Id)
                {
                    try
                    {
                        output = ProcessFileInternal(scenarioId, folder);
                    }
                    catch (Exception exception)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error processing Break Efficiency", exception));
                    }
                }
                else
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"Ignoring break efficiency because it should only be processed once for the run"));
                }
            }

            return output;
        }

        private ProcessBreakEfficiencyOutput ProcessFileInternal(Guid scenarioId, string folder)
        {
            var output = new ProcessBreakEfficiencyOutput();
            string breakEfficiencyFile = FileHelpers.GetPathToFileIfExists(folder, FileName);

            if (String.IsNullOrEmpty(breakEfficiencyFile))
            {
                return output;
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {breakEfficiencyFile}"));

            var demographicsById = Demographic.IndexListById(_dataSnapshot.AllDemographics.Value);
            var salesAreasIndexed = SalesArea.IndexListByCustomId(_dataSnapshot.AllSalesAreas.Value);

            // Create memory cache of Schedule documents
            var schedules = new List<ScheduleIndexed<Break, int>>();
            string lastScheduleDateKey = "";
            int count = 0;
            int countUpdated = 0;
            int countNotUpdated = 0;
            DateTime nextYield = DateTime.MinValue;

            // Process CSV in order of sales area + schedule date + schedule
            // time, allows us to keep as few Break/Schedule documents in memory
            var importSettings = CSVImportSettings.GetImportSettings(breakEfficiencyFile, typeof(BreakEfficiencyHeaderMap), typeof(BreakEfficiencyIndexMap));

            IBreakEfficiencyImportRepository breakEfficiencyImportRepository = new CSVBreakEfficiencyImportRepository(importSettings);
            IScheduleRepository scheduleRepository = null;

            var lazyScopeCreator = new Func<Lazy<IRepositoryScope>>(() => new Lazy<IRepositoryScope>(() => _repositoryFactory.BeginRepositoryScope()));
            var lazyScope = lazyScopeCreator();

            try
            {
                foreach (BreakEfficiencyImport breakEfficiencyImport in breakEfficiencyImportRepository.GetAll()
                    .OrderBy(be => be.sare_no).ThenBy(be => be.brek_sched_date).ThenBy(be => be.brek_nom_time))
                {
                    count++;
                    bool breakUpdated = false;
                    string scheduleDateKey = string.Format("{0}{1}{2}", breakEfficiencyImport.sare_no, (Char)9,
                        breakEfficiencyImport.brek_sched_date);

                    try
                    {
                        // Save changes if different sales area + schedule date
                        if (!String.IsNullOrEmpty(lastScheduleDateKey) && scheduleDateKey != lastScheduleDateKey) // No longer need previous Break/Schedule docs
                        {
                            if (scheduleRepository != null)
                            {
                                schedules.ForEach(scheduleIndexed => scheduleRepository.Add(scheduleIndexed.Schedule));
                                scheduleRepository.SaveChanges();
                            }

                            scheduleRepository = null;
                            lazyScope.Value.Dispose();
                            lazyScope = lazyScopeCreator();

                            // Clear data that we no longer need
                            schedules.Clear();
                        }

                        if (scheduleRepository == null)
                        {
                            scheduleRepository = lazyScope.Value.CreateRepository<IScheduleRepository>();
                        }

                        SalesArea salesArea = salesAreasIndexed.ContainsKey(breakEfficiencyImport.sare_no)
                            ? salesAreasIndexed[breakEfficiencyImport.sare_no]
                            : null;

                        // Load breaks if necessary
                        LoadBreaks(salesArea, breakEfficiencyImport, schedules, scheduleRepository);

                        // Update break efficiency
                        if (UpdateBreakEfficiency(salesArea, breakEfficiencyImport, schedules,
                            demographicsById))
                        {
                            breakUpdated = true;
                        }
                    }
                    catch (System.Exception exception)
                    {
                        // Log exception, continue
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                            string.Format("Error processing break efficiency for break no {0} (Record {1})",
                                breakEfficiencyImport.break_no, count), exception));
                    }
                    finally
                    {
                        lastScheduleDateKey = scheduleDateKey;
                        if (breakUpdated)
                        {
                            countUpdated++;
                        }
                        else
                        {
                            countNotUpdated++;
                        }

                        DoYield(false, ref nextYield);
                    }
                }

                // Save changed
                if (scheduleRepository != null)
                {
                    schedules.ForEach(scheduleIndexed => scheduleRepository.Add(scheduleIndexed.Schedule));
                    scheduleRepository.SaveChanges();
                }
            }
            finally
            {
                if (scheduleRepository != null)
                {
                    lazyScope.Value.Dispose();
                }
            }

            if (countUpdated > 0 && countNotUpdated == 0)   // Success, log info event
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Updated {0} break efficiencies (Not updated={1})", countUpdated, countNotUpdated)));
            }
            else          // Some updates failed, log warning event
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, string.Format("Updated {0} break efficiencies (Not updated={1})", countUpdated, countNotUpdated)));
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {breakEfficiencyFile}"));

            return output;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);

        /// <summary>
        /// Load breaks for break efficiency import record if not already in memory
        /// </summary>
        /// <param name="salesArea"></param>
        /// <param name="breakEfficiencyImport"></param>
        /// <param name="schedules"></param>
        private void LoadBreaks(SalesArea salesArea, BreakEfficiencyImport breakEfficiencyImport, List<ScheduleIndexed<Break, int>> schedules, IScheduleRepository scheduleRepository)
        {
            DateTime startDateTime = GetStartDateTime(breakEfficiencyImport);

            // Load Schedule document if necessary
            var schedule = schedules.Where(s => s.SalesArea == salesArea.Name && s.Date.Date == startDateTime.Date).FirstOrDefault();
            if (schedule == null)       // Not in memory, load it
            {
                var scheduleObject = scheduleRepository.GetSchedule(salesArea.Name, startDateTime.Date);
                if (scheduleObject != null)
                {
                    if (scheduleObject.Breaks != null && scheduleObject.Breaks.Count > 0)  // Schedule has breaks
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Break Efficiency processing. Schedule loaded for sales area {0} and date {1}", salesArea.Name, startDateTime.Date)));
                        scheduleObject.Breaks.ForEach(@break => @break.BreakEfficiencyList = null);
                    }
                    else    // No breaks
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, string.Format("Break Efficiency processing. Schedule document has no breaks for sales area {0} and date {1}", salesArea.Name, startDateTime.Date)));
                    }
                    schedules.Add(new ScheduleIndexed<Break, int>(scheduleObject, delegate (Break currentBreak) { return currentBreak; }, delegate (Break currentBreak) { return currentBreak.CustomId; }));
                }
                else    // No Schedule document, log warning
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, string.Format("Break Efficiency processing. Schedule document does not exist for sales area {0} and date {1}", salesArea.Name, startDateTime.Date)));
                }
            }
        }

        private DateTime GetStartDateTime(BreakEfficiencyImport breakEfficiencyImport)
        {
            // TODO: Make this shared...
            string nom_time = breakEfficiencyImport.brek_nom_time.ToString("000000");
            int seconds = (Convert.ToInt32(nom_time.Substring(0, 2)) * (60 * 60)) +     // Hours
                          (Convert.ToInt32(nom_time.Substring(2, 2)) * 60) +            // Mins
                          (Convert.ToInt32(nom_time.Substring(4, 2)));                  // Secs
            return DateHelper.GetDateTime(string.Format("{0} {1}", breakEfficiencyImport.brek_sched_date.ToString("00000000"), "000000"), "yyyyMMdd HHmmss", DateTimeKind.Utc).AddSeconds(seconds);
        }

        /// <summary> Updates break efficiency on both Break & Schedule
        /// documents </summary> <param name="salesArea"></param> <param
        /// name="breakEfficiencyImport"></param> <param name="schedules"></param>
        private bool UpdateBreakEfficiency(SalesArea salesArea, BreakEfficiencyImport breakEfficiencyImport, List<ScheduleIndexed<Break, int>> schedules, Dictionary<int, Demographic> demographicsById)
        {
            bool success = false;
            var startDateTime = GetStartDateTime(breakEfficiencyImport);

            // Set break efficiency on Schedule document
            var schedule = schedules.Where(s => s.SalesArea == salesArea.Name && s.Date == startDateTime.Date)
                .OrderByDescending(s => (s.BreaksByKey.Count))
                .FirstOrDefault();

            if (schedule != null)    // Schedule document found
            {
                var breakForSchedule = schedule.GetBreak(breakEfficiencyImport.break_no);
                if (breakForSchedule != null)   // Break found
                {
                    success = true;
                    var demographic = demographicsById.ContainsKey(breakEfficiencyImport.demo_no) ? demographicsById[breakEfficiencyImport.demo_no] : null;

                    if (breakForSchedule.BreakEfficiencyList == null)
                    {
                        breakForSchedule.BreakEfficiencyList = new List<BreakEfficiency>();
                    }

                    // TODO: Re-enable this when Break.Efficiency has been added
                    breakForSchedule.BreakEfficiencyList.Add(new BreakEfficiency(demographic.ExternalRef, breakEfficiencyImport.eff));
                }
            }
            return success;
        }

        /// <summary>
        /// Performs thread yield periodically or now if forced (We .Sleep
        /// rather than .Yield)
        /// </summary>
        /// <param name="force"></param>
        private static void DoYield(bool force, ref DateTime nextYield, int milliseconds = 1)
        {
            if (force || DateTime.Now >= nextYield)
            {
                Thread.Sleep(milliseconds);
                nextYield = DateTime.Now.AddMilliseconds(200);
            }
        }
    }
}
