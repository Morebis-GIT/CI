using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Microsoft.Extensions.Configuration;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Breaks
{
    /// <summary>
    /// Breaks
    /// </summary>
    public abstract class BreakSerializerBase : SerializerBase, IBreakSerializer
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConfiguration _applicationConfiguration;
        private readonly IMapper _mapper;
        private readonly IClock _clock;

        public BreakSerializerBase(
            IAuditEventRepository auditEventRepository,
            IFeatureManager featureManager,
            IRepositoryFactory repositoryFactory,
            IConfiguration applicationConfiguration,
            IMapper mapper,
            IClock clock)
            : base(auditEventRepository)
        {
            _auditEventRepository = auditEventRepository;
            _featureManager = featureManager;
            _repositoryFactory = repositoryFactory;
            _applicationConfiguration = applicationConfiguration;
            _mapper = mapper;
            _clock = clock;
        }

        public string Filename => "v_brek_list.xml";

        public void Serialize(
            string folderName,
            Run run,
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<Demographic> demographics,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            IReadOnlyCollection<Campaign> campaignWithProgrammeRestrictions,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            out IReadOnlyCollection<BreakWithProgramme> breaksWithProgrammes,
            out IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries)
        {
            RaiseInfo(
                $"Getting breaks with programmes, "
                + $"Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}"
            );

            breaksWithProgrammes = GetBreaksWithProgrammes(run, salesAreas);

            if (breaksWithProgrammes.Count == 0)
            {
                programmeDictionaries = Array.Empty<ProgrammeDictionary>();
                return;
            }

            var externalRefsFromBreak = breaksWithProgrammes
                .Where(p => !String.IsNullOrWhiteSpace(p.ProgrammeExternalreference))
                .Select(p => p.ProgrammeExternalreference);

            var externalRefsFromPrgm = scheduleProgrammes?.Where(p => !string.IsNullOrWhiteSpace(p.ExternalReference))
                .Select(p => p.ExternalReference);

            var externalRefsFromCampaigns = campaignWithProgrammeRestrictions != null && campaignWithProgrammeRestrictions.Any()
                ? campaignWithProgrammeRestrictions.SelectMany(c => c.ProgrammeRestrictions
                        .Where(p => p?.CategoryOrProgramme != null && p.CategoryOrProgramme.Any() &&
                                    p.IsCategoryOrProgramme.Equals(CategoryOrProgramme.P.ToString(),
                                        StringComparison.OrdinalIgnoreCase))
                        .SelectMany(p => p.CategoryOrProgramme))
                    .ToList()
                : Enumerable.Empty<string>();

            var combinedProgramRef = externalRefsFromBreak.Union(
                    externalRefsFromPrgm ?? Enumerable.Empty<string>()).Union(externalRefsFromCampaigns)
                .ToList();

            programmeDictionaries = combinedProgramRef.Count > 0
                ? FindProgrammeDictionaries(combinedProgramRef)
                : new List<ProgrammeDictionary>(0);

            RaiseInfo(
                $"Started populating {Filename}. Total breaks - {breaksWithProgrammes.Count}, "
                + $"Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}"
            );

            var exactBreaksRatingsTimeMatching =
                _featureManager.IsEnabled(nameof(ProductFeature.ExactBreaksRatingsTimeMatching));
            var useBreakPositionInProgram = _featureManager.IsEnabled(nameof(ProductFeature.UseBreakPositionInProgram));

            // This grouping for identify the break position within programme
            var breakGroups = breaksWithProgrammes
                .OrderBy(b => b.Break.ScheduledDate.Ticks)
                .GroupBy(d => new { d.Break.SalesArea, d.Break.ScheduledDate.Date, d.PrgtNo })
                .ToImmutableList();

            int totalCountBreaksWithPredictions = 0;
            int totalCountBreaksWithoutPredictions = 0;
            int timeDiffInMin = 0;

            var ratingsPredictionSchedules = GetRatingsPredictionSchedules(run, salesAreas);
            if (ratingsPredictionSchedules.Count > 0)
            {
                timeDiffInMin = ratingsPredictionSchedules.First()
                    .Ratings.Select(r => r.Time.TimeOfDay)
                    .GetTimeDiffInMin();
            }

            var prgDictionaries = programmeDictionaries;

            _ = Parallel.ForEach(breakGroups, subgrp =>
            {
                var ratingsPredictionSchedulesBySalesArea = ratingsPredictionSchedules
                    .Where(ratingsPredictionSchedule => ratingsPredictionSchedule.SalesArea == subgrp.Key.SalesArea)
                    .ToList();

                int breaksWithPredictions = 0;
                int breaksWithoutPredictions = 0;

                foreach (var breakWithProgramme in subgrp)
                {
                    var predictions = FindRatingPredictions(
                        ratingsPredictionSchedulesBySalesArea,
                        breakWithProgramme.Break.ScheduledDate,
                        exactBreaksRatingsTimeMatching,
                        timeDiffInMin);

                    breakWithProgramme.Predictions = predictions;
                    if (predictions is null || predictions.Count == 0)
                    {
                        breaksWithoutPredictions++;
                    }
                    else
                    {
                        breaksWithPredictions++;
                    }
                }

                _ = Interlocked.Add(ref totalCountBreaksWithPredictions, breaksWithPredictions);
                _ = Interlocked.Add(ref totalCountBreaksWithoutPredictions, breaksWithoutPredictions);
            });

            int uniqueid = 0;
            var runningListOfAgBreakLists = new ConcurrentBag<IReadOnlyCollection<AgBreak>>();

            _ = Parallel.ForEach(breakGroups, subgrp =>
            {
                var readOnlySubGroup = subgrp.ToImmutableList();

                int totalCountZeroBased = readOnlySubGroup.Count - 1;
                int pos = 0;
                var concurrentAgBreakList = new ConcurrentBag<AgBreak>();

                foreach (var breakWithProgramme in readOnlySubGroup)
                {
                    breakWithProgramme.ProgrammeNo = prgDictionaries.FirstOrDefault(p =>
                                                         p.ExternalReference.Equals(
                                                             breakWithProgramme.ProgrammeExternalreference,
                                                             StringComparison.OrdinalIgnoreCase))?.Id
                                                     ?? 0;

                    if (useBreakPositionInProgram)
                    {
                        breakWithProgramme.SetBreakPositionInProgram();
                    }
                    else
                    {
                        breakWithProgramme.SetPositionInProgram(totalCountZeroBased, pos);
                    }

                    pos++;

                    var programmeCategory = new List<int>();
                    var breakProgrammeCategories = breakWithProgramme.ProgrammeCategories?
                        .Select(categoryName => programmeCategories.FirstOrDefault(p => p.Name == categoryName))
                        .Where(c => c != null)
                        .ToList();
                    var parentCategories = new List<int>();

                    if (breakProgrammeCategories?.Any() == true)
                    {
                        foreach (var categoryWithParent in breakProgrammeCategories.Where(pg =>
                            !string.IsNullOrEmpty(pg.ParentExternalRef)))
                        {
                            var parent = programmeCategories.FirstOrDefault(pg =>
                                pg.ExternalRef == categoryWithParent.ParentExternalRef);
                            if (parent != null)
                            {
                                parentCategories.Add(parent.Id);
                            }
                        }

                        programmeCategory.AddRange(breakProgrammeCategories.Select(c => c.Id).Distinct());

                        if (parentCategories.Any())
                        {
                            programmeCategory.AddRange(parentCategories.Distinct());
                        }
                    }

                    int localUniqueid = Interlocked.Increment(ref uniqueid);

                    concurrentAgBreakList.Add(
                        _mapper.Map<AgBreak>(
                            Tuple.Create(
                                breakWithProgramme,
                                salesAreas,
                                demographics,
                                programmeCategory,
                                localUniqueid,
                                autoBookDefaultParameters.AgBreak)));
                }

                runningListOfAgBreakLists.Add(concurrentAgBreakList);
            });

            var agBreakList = new List<AgBreak>();
            foreach (var parallelResult in runningListOfAgBreakLists)
            {
                agBreakList.AddRange(parallelResult);
            }

            RaiseInfo(
                $"Breaks={breaksWithProgrammes.Count}, "
                + $"Breaks with predictions={totalCountBreaksWithPredictions}, "
                + $"Breaks with no predictions={totalCountBreaksWithoutPredictions}"
            );

            new AgBreaksSerialization().MapFrom(agBreakList).Serialize(Path.Combine(folderName, Filename));

            RaiseInfo(
                $"Finished populating {Filename}. Total breaks - {breaksWithProgrammes.Count}, "
                + $"Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}"
            );
        }

        /// <summary>
        /// Gets ratings predictions for each sales area, uses multiple threads
        /// </summary>
        protected virtual IReadOnlyCollection<RatingsPredictionSchedule> GetRatingsPredictionSchedules(Run run,
            IReadOnlyCollection<SalesArea> salesAreas)
        {
            var fromDateTime = run.StartDate;
            var toDateTime = DateHelper.ConvertBroadcastToStandard(run.EndDate, run.EndTime);

            List<RatingsPredictionSchedule> ratingsPredictionSchedules = new List<RatingsPredictionSchedule>();

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                String.Format("Getting ratings predictions for {0} sales areas ({1} - {2})", salesAreas.Count,
                    fromDateTime, toDateTime)));

            foreach (var salesArea in salesAreas)
            {
                {
                    List<RatingsPredictionSchedule> salesAreaRatingsPredictionSchedules =
                        new List<RatingsPredictionSchedule>();

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        String.Format("Gettings ratings predictions for sales area {0} ({1} - {2})", salesArea.Name,
                            fromDateTime, toDateTime)));

                    foreach (DateTime[] dateRange in DateHelper.GetDateRanges(fromDateTime, toDateTime, 30))
                    {
                        using (var scope = _repositoryFactory.BeginRepositoryScope())
                        {
                            var ratingsScheduleRepository = scope.CreateRepository<IRatingsScheduleRepository>();

                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                string.Format("Gettings batch of ratings predictions for sales area {0} ({1} - {2})",
                                    salesArea.Name, dateRange[0], dateRange[1])));

                            List<RatingsPredictionSchedule> rangeSalesAreaRatingsPredictionSchedules =
                                ratingsScheduleRepository.GetSchedules(dateRange[0], dateRange[1], salesArea.Name)
                                    .ToList();

                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                string.Format("Got batch of {0} ratings predictions for sales area {1} ({2} - {3})",
                                    RatingsPredictionSchedule.CountRatings(rangeSalesAreaRatingsPredictionSchedules),
                                    salesArea.Name, dateRange[0], dateRange[1])));

                            salesAreaRatingsPredictionSchedules.AddRange(rangeSalesAreaRatingsPredictionSchedules);
                        }
                    }

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        string.Format("Got {0} ratings predictions for sales area {1} ({2} - {3})",
                            RatingsPredictionSchedule.CountRatings(salesAreaRatingsPredictionSchedules), salesArea.Name,
                            fromDateTime, toDateTime)));

                    ratingsPredictionSchedules.AddRange(salesAreaRatingsPredictionSchedules);
                }
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                string.Format("Got {0} ratings predictions for {1} sales areas ({2} - {3})",
                    RatingsPredictionSchedule.CountRatings(ratingsPredictionSchedules), salesAreas.Count, fromDateTime,
                    toDateTime)));

            return ratingsPredictionSchedules;
        }

        protected abstract IReadOnlyCollection<BreakWithProgramme> GetBreaksWithProgrammes(
            Run run,
            IReadOnlyCollection<SalesArea> salesAreas);

        protected virtual string ExcludeBreakType => _applicationConfiguration["AutoBooks:ExcludeBreakType"];

        protected IReadOnlyCollection<ProgrammeDictionary> FindProgrammeDictionaries(List<string> externalRefs)
        {
            if (externalRefs == null || externalRefs.Count == 0)
            {
                return null;
            }

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var programmeDictRepository = scope.CreateRepository<IProgrammeDictionaryRepository>();

                externalRefs = externalRefs.Distinct(StringComparer.OrdinalIgnoreCase).Trim().ToList();
                return programmeDictRepository.FindByExternal(externalRefs).ToArray();
            }
        }

        protected List<Rating> FindRatingPredictions(
            IReadOnlyCollection<RatingsPredictionSchedule> ratingsPredictionsSchedules,
            DateTime breakScheduleDate,
            bool exactBreaksRatingsTimeMatching,
            int timeDiffInMin)
        {
            var ratingsPredictionList = new List<Rating>();

            if (timeDiffInMin == 0
                || ratingsPredictionsSchedules == null
                || ratingsPredictionsSchedules.Count == 0
                || !ratingsPredictionsSchedules.Any(i => i.Ratings.Count > 0))
            {
                return null;
            }

            var timeSlot = exactBreaksRatingsTimeMatching
                ? breakScheduleDate //timeSlot equals break time if we need exact match
                : CalculateNearestTimeSlot(breakScheduleDate, timeDiffInMin);

            foreach (RatingsPredictionSchedule ratingsPredictionsSchedule in ratingsPredictionsSchedules)
            {
                ratingsPredictionList.AddRange(
                    ratingsPredictionsSchedule.Ratings.Where(rating => rating.Time.Equals(timeSlot)));
            }

            return ratingsPredictionList;
        }

        protected DateTime CalculateNearestTimeSlot(DateTime breakSchduledDate, int timeDiffInMinutes)
        {
            var timeInSec = breakSchduledDate.TimeOfDay.TotalSeconds;
            var nearestTimeInSec = Math.Truncate(timeInSec / (timeDiffInMinutes * 60)) * timeDiffInMinutes * 60;
            return breakSchduledDate.Date.AddSeconds(nearestTimeInSec);
        }
    }
}
